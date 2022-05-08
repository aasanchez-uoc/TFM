using GraphProcessor;
using Poly2Tri;
using StraightSkeletonNet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Geometry/Transformations/Basic Roof")]

public class BasicRoof : GeometryFlowBaseNode
{

    [Input, ShowAsDrawer]
    public float Height = 1;

    [Output("Output flow", allowMultiple = false)]
    public new GeometryFlow OutputFlow;


    public override void Process(GraphFlow inputflow)
    {
        GeometryFlow InputFlow = inputflow as GeometryFlow;
        if (InputFlow?.CurrentGameObject != null)
        {
            List<Vector3> meshVertices = new List<Vector3>();
            List<Face> meshFaces = new List<Face>();

            foreach (Face face in InputFlow.Mesh.faces)
            {
                HashSet<Vertex> originalFaceVertices = new HashSet<Vertex>();
                var edges = WingedEdge.SortEdgesByAdjacency(face);

                foreach (Edge edge in edges)
                {
                    originalFaceVertices.UnionWith(InputFlow.Mesh.GetVertices(new List<int> { edge.a, edge.b }));
                }

                var poly = new List<StraightSkeletonNet.Primitives.Vector2d>();

                foreach (Vertex vertex in originalFaceVertices)
                {
                    var point2d = WorldPointToPlane(vertex.position, face);
                    poly.Add(new StraightSkeletonNet.Primitives.Vector2d(point2d.x, point2d.y));
                }
                Skeleton skeleton = SkeletonBuilder.Build(poly);

                //build new edges and vertices from skeleton
                HashSet<Vertex> newVertices = new HashSet<Vertex>();

                foreach (EdgeResult edgeResult in skeleton.Edges)
                {
                    List<int> faceIndexes = new List<int>();
                    List<Vector3> faceVertices = new List<Vector3>();
                    foreach (var v in edgeResult.Polygon)
                    {
                        float extrude = (poly.Contains(v)) ? 0 : Height;
                        Vector3 vertex = PlanePointToWorld(v, extrude, face, InputFlow.Mesh);
                        if (!meshVertices.Contains(vertex)) meshVertices.Add(vertex);
                        faceIndexes.Add(meshVertices.IndexOf(vertex));
                        faceVertices.Add(vertex);
                    }
                    if (TriangulationUtils.TriangulateVertices(faceVertices.ToArray(), faceIndexes, out List<int> triangles))
                    {
                        Face f = new Face(triangles);
                        f.Reverse();
                        meshFaces.Add(f);
                    }
                }

            }

            InputFlow.Mesh.RebuildWithPositionsAndFaces(meshVertices, meshFaces);
            InputFlow.Mesh.ToTriangles(meshFaces);
            InputFlow.Mesh.ToMesh();
            InputFlow.Mesh.Refresh();
            OutputFlow = InputFlow;

        }
    }


        private Vector2 WorldPointToPlane(Vector3 p, Face planeFace)
    {
        return new Vector2(p.x, p.z);
    }
    private Vector3 PlanePointToWorld(StraightSkeletonNet.Primitives.Vector2d p, float extrudeHeight, Face planeFace, ProBuilderMesh mesh)
    {
        return PlanePointToWorld((float)p.X, (float)p.Y, extrudeHeight, planeFace, mesh);
    }
    private Vector3 PlanePointToWorld(float x, float y, float extrudeHeight, Face planeFace, ProBuilderMesh mesh)
    {
        var planeY = mesh.GetVertices(planeFace.distinctIndexes)[0].position.y;
        return new Vector3(x, planeY + extrudeHeight, y);
    }
}
