using GraphProcessor;
using Poly2Tri;
using StraightSkeletonNet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ProBuilder;
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

    protected override void Process(int index)
    {
        if (InputFlows == null) return;
        GeometryFlow InputFlow = InputFlows.ToList()[index];
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
                    originalFaceVertices.UnionWith(InputFlow.Mesh.GetVertices(new List<int> { edge.a , edge.b}));
                }

                var poly = new List<StraightSkeletonNet.Primitives.Vector2d>();

                foreach(Vertex vertex in originalFaceVertices)
                {
                    var point2d = WorldPointToPlane(vertex.position, face);
                    poly.Add(new StraightSkeletonNet.Primitives.Vector2d(point2d.x, point2d.y));
                }
                Skeleton skeleton = SkeletonBuilder.Build(poly);

                //build new edges and vertices from skeleton
                HashSet<Vertex> newVertices = new HashSet<Vertex>();

                foreach(EdgeResult edgeResult in skeleton.Edges)
                {
                    List<int> faceIndexes = new List<int>();
                    List<Vector3> faceVertices = new List<Vector3>();
                    foreach (var v in edgeResult.Polygon)
                    {
                        float extrude = (poly.Contains(v)) ? 0 : Height;
                        Vector3 vertex = PlanePointToWorld(v, extrude ,face, InputFlow.Mesh);
                        if (!meshVertices.Contains(vertex)) meshVertices.Add(vertex);
                        faceIndexes.Add(meshVertices.IndexOf(vertex));
                        faceVertices.Add(vertex);
                    }
                    if ( TriangulateVertices(faceVertices.ToArray(), faceIndexes, out List<int> triangles))
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

    public static bool TriangulateVertices(Vector3[] vertices, List<int> indices, out List<int> triangles, Vector3[][] holes = null)
    {
        triangles = null;
        int vertexCount = vertices == null ? 0 : vertices.Length;

        if (vertexCount < 3)
            return false;

        var normal = Projection.FindBestPlane(vertices).normal;
        Vector2[] points2d = Projection.PlanarProject(vertices, null, normal);
        Vector2[][] holes2d = null;
        if (holes != null)
        {
            holes2d = new Vector2[holes.Length][];
            for (int i = 0; i < holes.Length; i++)
            {
                if (holes[i].Length < 3)
                    return false;

                holes2d[i] = Projection.PlanarProject(holes[i], null, normal);
            }
        }

        return Triangulate(points2d, indices, holes2d, out triangles);
    }

    public static bool Triangulate(IList<Vector2> points, List<int> allIndexes, IList<IList<Vector2>> holes, out List<int> trianglesIndexes)
    {
        trianglesIndexes = new List<int>();
        List<int>  localIndexes = new List<int>();
        var allPoints = new List<Vector2>(points);

        Polygon polygon = new Polygon(points.Select(x => new PolygonPoint(x.x, x.y)));
        if (holes != null)
        {
            for (int i = 0; i < holes.Count; i++)
            {
                allPoints.AddRange(holes[i]);
                var holePolgyon = new Polygon(holes[i].Select(x => new PolygonPoint(x.x, x.y)));
                polygon.AddHole(holePolgyon);
            }
        }

        try
        {
            TriangulationContext triangulationContext = new DTSweepContext();
            triangulationContext.Clear();
            triangulationContext.PrepareTriangulation(polygon);
            DTSweep.Triangulate((DTSweepContext)triangulationContext);
        }
        catch (System.Exception e)
        {
            return false;
        }

        foreach (DelaunayTriangle d in polygon.Triangles)
        {
            int index1 = points.IndexOf(new Vector2((float)d.Points[0].X, (float)d.Points[0].Y));
            int index2 = points.IndexOf(new Vector2((float)d.Points[1].X, (float)d.Points[1].Y));
            int index3 = points.IndexOf(new Vector2((float)d.Points[2].X, (float)d.Points[2].Y));
            if (index1 < 0 || index2 < 0 || index3 < 0 || System.Math.Max(index1, System.Math.Max(index2, index3)) >= allIndexes.Count)
            {
                return false;
            }

            localIndexes.Add(index1);
            localIndexes.Add(index2);
            localIndexes.Add(index3);

            trianglesIndexes.Add(allIndexes[index1]);
            trianglesIndexes.Add(allIndexes[index2]);
            trianglesIndexes.Add(allIndexes[index3]);
        }

        WindingOrder originalWinding = SurfaceTopology.GetWindingOrder(points);

        // if the re-triangulated first tri doesn't match the winding order of the original
        // vertices, flip 'em

        if (SurfaceTopology.GetWindingOrder(new Vector2[3]
        {
                allPoints[localIndexes[0]],
                allPoints[localIndexes[1]],
                allPoints[localIndexes[2]],

        }) != originalWinding)
        {
            trianglesIndexes.Reverse();
        }
            

        return true;
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
