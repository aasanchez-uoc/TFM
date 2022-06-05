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
        catch
        {
            return null;
        }
        return polygon.Triangles.ToList();
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
