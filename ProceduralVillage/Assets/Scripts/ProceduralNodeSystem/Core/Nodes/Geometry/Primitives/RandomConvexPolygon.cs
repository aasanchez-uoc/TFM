using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

/// <summary>
/// Genera un polígono convexo, especificando el número de vértices y tamaño (ancho y largo).
/// </summary>
[System.Serializable, NodeMenuItem("Geometry/Primitives/Random Convex Polygon")]
public class RandomConvexPolyNode : BaseFlowNode
{
    [Input, ShowAsDrawer]
    public float MaxWidth = 1;

    [Input, ShowAsDrawer]
    public float MaxHeight = 1;

    [Input, ShowAsDrawer]
    public int NumberOfVertices = 4;

    public string ObjectName = "Convex Polygon";

    public override void Process(GraphFlow InputFlow)
    {
        if (InputFlow?.CurrentGameObject != null)
        {
            if (MaxWidth < 1 || MaxHeight < 1 || NumberOfVertices < 3) return;
            var points2D = generateRandomConvexPolygon(NumberOfVertices, MaxWidth, MaxHeight);
            List<Vector3> points = points2D.Select(p => new Vector3(p.x, 0, p.y)).ToList();
            ProBuilderMesh m_Mesh = ProBuilderMesh.Create();
            m_Mesh.CreateShapeFromPolygon(points, 0, false);


            m_Mesh.SetMaterial(m_Mesh.faces, BuiltinMaterials.defaultMaterial);
            m_Mesh.transform.parent = InputFlow.CurrentGameObject.transform;
            m_Mesh.ToMesh();
            m_Mesh.Refresh();
            OutputFlow = new GraphFlow(ObjectName, m_Mesh.gameObject, InputFlow.CurrentGameObject.transform);
        }
    }

    //c# port of https://cglab.ca/~sander/misc/ConvexGeneration/ValtrAlgorithm.java
    private static List<Vector2> generateRandomConvexPolygon(int n, float w, float h)
    {
        System.Random rand = new System.Random();

        // Generate two lists of random X and Y coordinates
        List<float> xPool = new List<float>(n);
        List<float> yPool = new List<float>(n);

        for (int i = 0; i < n; i++)
        {
            xPool.Add((float) rand.NextDouble() * w);
            yPool.Add((float)rand.NextDouble() * h);
        }

        // Sort them
        xPool.Sort();
        yPool.Sort();

        // Isolate the extreme points
        float minX = xPool[0];
        float maxX = xPool[n - 1];
        float minY = yPool[0];
        float maxY = yPool[n - 1];

        // Divide the interior points into two chains & Extract the vector components
        List<float> xVec = new List<float>(n);
        List<float> yVec = new List<float>(n);

        float lastTop = minX, lastBot = minX;

        for (int i = 1; i < n - 1; i++)
        {
            float xPos = xPool[i];

            if (rand.Next(2) == 0)
            {
                xVec.Add(xPos - lastTop);
                lastTop = xPos;
            }
            else
            {
                xVec.Add(lastBot - xPos);
                lastBot = xPos;
            }
        }

        xVec.Add(maxX - lastTop);
        xVec.Add(lastBot - maxX);

        float lastLeft = minY, lastRight = minY;

        for (int i = 1; i < n - 1; i++)
        {
            float yPos = yPool[i];

            if (rand.Next(2) == 0)
            {
                yVec.Add(yPos - lastLeft);
                lastLeft = yPos;
            }
            else
            {
                yVec.Add(lastRight - yPos);
                lastRight = yPos;
            }
        }

        yVec.Add(maxY - lastLeft);
        yVec.Add(lastRight - maxY);

        // Randomly pair up the X- and Y-components;
        yVec = yVec.OrderBy(item => rand.Next()).ToList();

        // Combine the paired up components into vectors
        List<Vector2> vec = new List<Vector2>(n);

        for (int i = 0; i < n; i++)
        {
            vec.Add(new Vector2(xVec[i], yVec[i]));
        }

        // Sort the vectors by angle
        vec = vec.OrderBy( v => Mathf.Atan2(v.y, v.x)).ToList();

        // Lay them end-to-end
        float x = 0, y = 0;
        float minPolygonX = 0;
        float minPolygonY = 0;
        List<Vector2> points = new List<Vector2>(n);

        for (int i = 0; i < n; i++)
        {
            points.Add(new Vector2(x, y));

            x += vec[i].x;
            y += vec[i].y;

            minPolygonX = Mathf.Min(minPolygonX, x);
            minPolygonY = Mathf.Min(minPolygonY, y);
        }

        // Move the polygon to the original min and max coordinates
        float xShift = minX - minPolygonX;
        float yShift = minY - minPolygonY;

        for (int i = 0; i < n; i++)
        {
            Vector2 p = points[i];
            points[i] = new Vector2(p.x + xShift, p.y + yShift);
        }

        return points;
    }
}
