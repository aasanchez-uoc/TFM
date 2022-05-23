using GraphProcessor;
using Habrador_Computational_Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Geometry/Voronoi From Mesh")]
public class VoronoiFromMeshNode : BaseFlowNode
{
    [Input, ShowAsDrawer]
    public int NumberOfPoints = 2;

    [Output("Voronoi Outputs", allowMultiple = false)]
    public new List<GraphFlow> OutputFlow;

    [Output("Original Mesh", allowMultiple = false)]
    public GraphFlow OriginalMeshFlow;

    public string ObjectsNames = "Voronoi Cell";
    public override void Process(GraphFlow InputFlow)
    {
   
        OutputFlow = new List<GraphFlow>();
        if (InputFlow?.CurrentGameObject != null)
        {
            ProBuilderMesh Mesh = InputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>();
            if (Mesh != null)
            {
                MeshFilter filter = Mesh.gameObject.GetComponent<MeshFilter>();
                List<Vector3> Points = GenerateRandomPointsOnMesh(filter.sharedMesh, NumberOfPoints);

                if (Points.Count != 0)
                {
                    float maxX = Points.Max(p => Mathf.Abs(p.x)) * 5;
                    float maxZ = Points.Max(p => Mathf.Abs(p.z)) * 5;
                    Points.Add(new Vector3(0f, 0f, maxZ));
                    Points.Add(new Vector3(0f, 0f, -maxZ));
                    Points.Add(new Vector3(maxX, 0f, 0f));
                    Points.Add(new Vector3(-maxX, 0f, 0f));

                }

                HashSet<MyVector2> sites_2d = new HashSet<MyVector2>();

                foreach (Vector3 v in Points)
                {
                    sites_2d.Add(v.ToMyVector2());
                }

                //Normalize
                Normalizer2 normalizer = new Normalizer2(new List<MyVector2>(sites_2d));
                HashSet<MyVector2> randomSites_2d_normalized = normalizer.Normalize(sites_2d);


                //Generate the voronoi
                HashSet<VoronoiCell2> voronoiCells = _Voronoi.DelaunyToVoronoi(randomSites_2d_normalized);

                //Unnormalize
                voronoiCells = normalizer.UnNormalize(voronoiCells);

                List<MyVector2> clipPolygon = Mesh.GetVertices().Select(v => v.position.ToMyVector2()).ToList();

                int num = 1;
                foreach (VoronoiCell2 face in voronoiCells)
                {

                    List<MyVector2> points2d = vertexListFromEdges(face.edges);

                    if (points2d != null)
                    {
                        List<MyVector2> clippedPoints2d = SutherlandHodgman.ClipPolygon(points2d, clipPolygon);
                        List<Vector3> points = clippedPoints2d.Select(p => p.ToVector3(0)).ToList();
                        if (points.Count > 0)
                        {
                            ProBuilderMesh m_Mesh = ProBuilderMesh.Create();
                            m_Mesh.CreateShapeFromPolygon(points, 0, false);
                            m_Mesh.SetMaterial(m_Mesh.faces, BuiltinMaterials.defaultMaterial);
                            m_Mesh.ToMesh();
                            m_Mesh.Refresh();

                            GraphFlow output = new GraphFlow(ObjectsNames + " " + num, m_Mesh.gameObject, InputFlow.CurrentGameObject.transform);

                            num++;
                            OutputFlow.Add(output);
                        }
                    }
                }
                OriginalMeshFlow = InputFlow;
            }
        }
    }

    private List<Vector3> GenerateRandomPointsOnMesh(Mesh mesh,int numberOfPoints)
    {
        List<Vector3> list = new List<Vector3>();
        for(int i = 0; i < numberOfPoints; i++)
        {
            Vector3? p = MeshUtils.GetRandomPointOnMesh(mesh);
            if(p != null) list.Add((Vector3)p);
        }

        return list;
    }


    private List<MyVector2> vertexListFromEdges(List<VoronoiEdge2> edges)
    {
        List<MyVector2> points = new List<MyVector2>();
        HashSet<VoronoiEdge2> unusedEdges = new HashSet<VoronoiEdge2>(edges);
        VoronoiEdge2 startingEdge = unusedEdges.First();
        VoronoiEdge2 selectedEdge = startingEdge;
        bool backwards = false;

        while (unusedEdges.Count > 0)
        {
            if (!backwards)
            {
                if (! points.Any(p => EqualPoints(p, selectedEdge.p1)) ) points.Add(selectedEdge.p1);
                unusedEdges.Remove(selectedEdge);
                MyVector2 p2 = selectedEdge.p2;
                bool found = false;
                foreach (VoronoiEdge2 edge in unusedEdges)
                {
                    if(EqualPoints(edge.p1, p2))
                    {
                        selectedEdge = edge;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    if (!points.Any(p => EqualPoints(p, selectedEdge.p2))) points.Add(selectedEdge.p2);
                    backwards = true;
                    selectedEdge = startingEdge;
                }
            }
            else
            {
                bool found = false;
                MyVector2 p1 = selectedEdge.p1;
                foreach (VoronoiEdge2 edge in unusedEdges)
                {
                    if (EqualPoints(edge.p2, p1))
                    {
                        selectedEdge = edge;
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    if (!points.Any(p => EqualPoints(p, selectedEdge.p2))) points.Insert(0, selectedEdge.p2);
                    unusedEdges.Remove(selectedEdge);
                }
                else
                {
                    points = null;
                    break; //couldnt fix it, we stop
                }
            }

        }
        return points;
    }

    bool EqualPoints(MyVector2 p1, MyVector2 p2)
    {
        return MyVector2.SqrDistance(p1, p2) < MathUtility.EPSILON;
    }
}
