using GraphProcessor;
using Habrador_Computational_Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Geometry/Experimental/Voronoi From Points")]
public class VoronoiFromPointsNode : BaseFlowNode
{
    [Input]
    public List<Vector3> Points;

    [Output("Output Flow", allowMultiple = false)]
    public new List<GraphFlow> OutputFlow;


    public override void Process(GraphFlow InputFlow)
    {
        OutputFlow = new List<GraphFlow>();
        if (InputFlow?.CurrentGameObject != null)
        {         
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

            foreach (VoronoiCell2 face in voronoiCells)
            {               
                List<Vector3> points = new List<Vector3>();
                HashSet<VoronoiEdge2> unusedEdges = new HashSet<VoronoiEdge2>(face.edges);
                VoronoiEdge2 startingEdge = unusedEdges.First();
                VoronoiEdge2 selectedEdge = startingEdge;
                bool backwards = false;

                while (unusedEdges.Count > 0)
                {
                    if(!backwards)
                    {
                        Vector3 p1 = new Vector3(selectedEdge.p1.x, 0, selectedEdge.p1.y);
                        if (!ContainsPoint(p1, points)) points.Add(p1);
                        unusedEdges.Remove(selectedEdge);
                        MyVector2 p2 = selectedEdge.p2;
                        bool found = false;
                        foreach (VoronoiEdge2 edge in unusedEdges)
                        {
                            if (edge.p1.Equals(p2))
                            {
                                selectedEdge = edge;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            Vector3 p2_3d = new Vector3(selectedEdge.p2.x, 0, selectedEdge.p2.y);
                            if (!ContainsPoint(p2_3d, points)) points.Add(p2_3d);
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
                            if (edge.p2.Equals(p1))
                            {
                                selectedEdge = edge;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            Vector3 p2 = new Vector3(selectedEdge.p2.x, 0, selectedEdge.p2.y);
                            if (!ContainsPoint(p2, points)) points.Insert(0, p2);
                            unusedEdges.Remove(selectedEdge);
                        }
                        else
                        {
                            break; //couldnt fix it, we stop
                        }
                    }

                }

                ProBuilderMesh m_Mesh = ProBuilderMesh.Create();
                m_Mesh.CreateShapeFromPolygon(points, 0, false);
                m_Mesh.SetMaterial(m_Mesh.faces, BuiltinMaterials.defaultMaterial);
                m_Mesh.transform.parent = InputFlow.CurrentGameObject.transform;
                m_Mesh.ToMesh();
                m_Mesh.Refresh();

                GraphFlow output = new GraphFlow();
                output.CurrentGameObject = m_Mesh.gameObject;
                output.CurrentGameObject.transform.parent = InputFlow.CurrentGameObject.transform;
                OutputFlow.Add(output);
            }
        }
    }

    private bool ContainsPoint(Vector3 p1, List<Vector3> points)
    {
        foreach(Vector3 p in points)
        {
            if (p == p1) return true;
        }
        return false;
    }
}
