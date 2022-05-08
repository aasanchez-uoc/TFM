using Poly2Tri;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public static class TriangulationUtils
{
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
        List<int> localIndexes = new List<int>();
        var triangles = GetTriangulation(points, holes, out trianglesIndexes, out List<Vector2> allPoints);
        if (triangles == null) return false;
        foreach (DelaunayTriangle d in triangles)
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


    private static List<DelaunayTriangle> GetTriangulation(IList<Vector2> points, IList<IList<Vector2>> holes, out List<int> trianglesIndexes, out List<Vector2> allPoints)
    {
        trianglesIndexes = new List<int>();
        allPoints = new List<Vector2>(points);

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
            return null;
        }
        return polygon.Triangles.ToList();
    }

    public static bool VoronoiFromVertices(Vector3[] vertices, List<int> indices, out List<Face> faces, out List<Vector2> finalPoints  ,Vector3[][] holes = null)
    {
        faces = null;
        finalPoints = null;
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

        return Voronoi(points2d, indices, holes2d, out faces, out finalPoints);
    }
    public static bool VoronoiFacesFromPoints(Vector3[] vertices, List<int> indices, out List<List<Vector2>> polyFaces, Vector3[][] holes = null)
    {
        polyFaces = null;
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

        return VoronoiPoly(points2d, indices, holes2d, out polyFaces);
    }

    public static bool Voronoi(IList<Vector2> points, List<int> allIndexes, IList<IList<Vector2>> holes, out List<Face> faces, out List<Vector2> finalPoints)
    {
        faces = null;
        finalPoints = null;
        var triangles = GetTriangulation(points, holes, out List<int> trianglesIndexes, out List<Vector2> allPoints);
        if (triangles == null) return false;
        faces = GenerateFacesFromDelaunay(points.ToList(), triangles, out finalPoints).ToList();
        return true;
    }
    public static bool VoronoiPoly(IList<Vector2> points, List<int> allIndexes, IList<IList<Vector2>> holes, out List<List<Vector2>> faces)
    {
        faces = null;
        var triangles = GetTriangulation(points, holes, out List<int> trianglesIndexes, out List<Vector2> allPoints);
        if (triangles == null) return false;
        faces = GeneratePolyFacesFromDelaunay(points.ToList(), triangles).ToList();
        return true;
    }

    private static IEnumerable<Face> GenerateFacesFromDelaunay(List<Vector2> points, IEnumerable<DelaunayTriangle> triangulation, out List<Vector2> finalPoints)
    {
        var voronoiFaces = new HashSet<Face>();
        finalPoints = new List<Vector2>();

        foreach (Vector2 point in points)
        {
            List<int> faceIndexs = new List<int>();
            var faceEdges = new List<SimpleEdge>();

            foreach (DelaunayTriangle triangle in triangulation)
            {
                if(TriangulationPointToVector2(triangle.Points[0]) == point || TriangulationPointToVector2(triangle.Points[1]) == point || TriangulationPointToVector2(triangle.Points[2]) == point)
                {
                    Vector2 vertexA = GetCircumcenter(triangle);

                    foreach(DelaunayTriangle neighbor in triangle.Neighbors)
                    {
                        if (neighbor != null && (TriangulationPointToVector2(neighbor.Points[0]) == point || TriangulationPointToVector2(neighbor.Points[1]) == point || TriangulationPointToVector2(neighbor.Points[2]) == point))
                        {
                            Vector2 vertexB = GetCircumcenter(neighbor);
                            SimpleEdge edge = new SimpleEdge(vertexA, vertexB);
                            SimpleEdge edge2 = new SimpleEdge(vertexB, vertexA);
                            if (!faceEdges.Contains(edge) && !faceEdges.Contains(edge2))
                            {
                                faceEdges.Add(edge);

                                if (!finalPoints.Contains(vertexA)) finalPoints.Add(vertexA);
                                if (!finalPoints.Contains(vertexB)) finalPoints.Add(vertexB);
                                if (!finalPoints.Contains(point)) finalPoints.Add(point);

                                int indexA = finalPoints.IndexOf(vertexA);
                                int indexB = finalPoints.IndexOf(vertexB);
                                int indexC = finalPoints.IndexOf(point);

                               
                                faceIndexs.Add(indexA);
                                faceIndexs.Add(indexC);
                                faceIndexs.Add(indexB);

                            }
                        }
                    }
                }
            }
            if(faceIndexs?.Count > 0)
            {
                Face face = new Face(faceIndexs);
                voronoiFaces.Add(face);
            }
        }
        return voronoiFaces;
    }

    private static List<List<Vector2>> GeneratePolyFacesFromDelaunay(List<Vector2> points, IEnumerable<DelaunayTriangle> triangulation)
    {
        var voronoiFaces = new List<List<Vector2>>();

        foreach (Vector2 point in points)
        {
            List<Vector2> faceVertices = new List<Vector2>();
            var faceEdges = new List<SimpleEdge>();

            foreach (DelaunayTriangle triangle in triangulation)
            {
                if (TriangulationPointToVector2(triangle.Points[0]) == point || TriangulationPointToVector2(triangle.Points[1]) == point || TriangulationPointToVector2(triangle.Points[2]) == point)
                {
                    Vector2 vertexA = GetCircumcenter(triangle);

                    foreach (DelaunayTriangle neighbor in triangle.Neighbors)
                    {
                        if (neighbor != null && (TriangulationPointToVector2(neighbor.Points[0]) == point || TriangulationPointToVector2(neighbor.Points[1]) == point || TriangulationPointToVector2(neighbor.Points[2]) == point))
                        {
                            Vector2 vertexB = GetCircumcenter(neighbor);
                            SimpleEdge edge = new SimpleEdge(vertexA, vertexB);;
                            if (!faceEdges.Contains(edge))
                            {
                                faceEdges.Add(edge);
                                if (!faceVertices.Contains(vertexA)) faceVertices.Add(vertexA);
                                if (!faceVertices.Contains(vertexB)) faceVertices.Add(vertexB);

                            }
                        }
                    }
                }
            }
            if (faceVertices?.Count > 2)
            {
                voronoiFaces.Add(faceVertices);
            }
        }
        return voronoiFaces;
    }

    internal static Vector2 TriangulationPointToVector2(TriangulationPoint p)
    {
        return new Vector2((float)p.X, (float)p.Y);
    }

    public static Vector2 GetCircumcenter(DelaunayTriangle triangle)
    {

        TriangulationPoint p0 = triangle.Points[0];
        TriangulationPoint p1 = triangle.Points[1];
        TriangulationPoint p2 = triangle.Points[2];
        var dA = p0.X * p0.X + p0.Y * p0.Y;
        var dB = p1.X * p1.X + p1.Y * p1.Y;
        var dC = p2.X * p2.X + p2.Y * p2.Y;

        var aux1 = (dA * (p2.Y - p1.Y) + dB * (p0.Y - p2.Y) + dC * (p1.Y - p0.Y));
        var aux2 = -(dA * (p2.X - p1.X) + dB * (p0.X - p2.X) + dC * (p1.X - p0.X));
        var div = (2 * (p0.X * (p2.Y - p1.Y) + p1.X * (p0.Y - p2.Y) + p2.X * (p1.Y - p0.Y)));

        return new Vector2((float)(aux1 / div), (float)(aux2 / div));

    }

    public class SimpleEdge : IEquatable<SimpleEdge>
    {
        public Vector2 a;
        public Vector2 b;

        public SimpleEdge(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
        }

        public bool Equals(SimpleEdge other)
        {
            return (this.a.Equals(other.a) && this.b.Equals(other.b)) || (this.a.Equals(other.b) && this.b.Equals(other.a));
        }
    }

}
