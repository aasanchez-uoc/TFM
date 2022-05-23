using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Geometry/Transformations/SplitTest")]
public class SplitTest : BaseFlowNode
{
    [Output("Selected Faces", allowMultiple = false)]
    public new GraphFlow OutputFlow;

    public SplitDirection splitDirection;

    Dictionary<Vertex, SplitSide> vertexSides;
    Dictionary<Edge, SplitSide> edgeSides;
    Dictionary<Face, SplitSide> faceSides;

    public override void Process(GraphFlow InputFlow)
    {
        if (InputFlow?.CurrentGameObject != null)
        {
            ProBuilderMesh Mesh = InputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>();
            if (Mesh != null)
            {
                ApplySplit(splitDirection, Mesh);
                OutputFlow = InputFlow;
                Mesh.ToMesh();
                Mesh.Refresh();
            }
        }
    }


        void ApplySplit(SplitDirection dirName, ProBuilderMesh mesh, float distance = 0.50f)
    {
              
        Vector3 dir = DirectionNameToVecto3(dirName);
 

        var r = mesh.GetComponent<Renderer>();
        if (r == null)
            return;

        float dirSize;
        if (dirName == SplitDirection.X) dirSize = -r.bounds.size.x/2 + r.bounds.center.x ;
        else if (dirName == SplitDirection.Y) dirSize = -r.bounds.size.y/2 + r.bounds.center.y;
        else dirSize = -r.bounds.size.z/2 + r.bounds.center.z ;

        Plane splitPlane = new Plane(dir, distance + dirSize);

        HashSet<Edge> edgesToSplit = new HashSet<Edge>();
        List<Face> facesToSplit = new List<Face>();

        vertexSides = new Dictionary<Vertex, SplitSide>();
        edgeSides = new Dictionary<Edge, SplitSide>();
        faceSides = new Dictionary<Face, SplitSide>();


        //Vertex classification
        foreach (Vertex vertex in mesh.GetVertices())
        {
            if (isPointOnPlane(splitPlane, mesh.transform.TransformPoint(vertex.position)))
            {
                vertexSides[vertex] = SplitSide.on;
            }
            else if (isPointAbovePlane(splitPlane, mesh.transform.TransformPoint(vertex.position)))
            {
                vertexSides[vertex] = SplitSide.above;
            }
            else
            {
                vertexSides[vertex] = SplitSide.below;
            }
        }

        ////Face classification
        foreach (Face face in mesh.faces)
        {
            //bool isOn = true;
            //bool isAbove = true;
            //bool isBelow = true;
            foreach (Edge edge in face.edges)
            {
                UpdateEdgeClassification(mesh, edge);
                if (edgeSides.TryGetValue(edge, out SplitSide side))
                {
                    if (side == SplitSide.above)
                    {
                        //isOn = false;
                        //isBelow = false;
                    }
                    else if (side == SplitSide.below)
                    {
                        //isOn = false;
                        //isAbove = false;
                    }
                    else if (side == SplitSide.intersect || side == SplitSide.on)
                    {
                        //isOn = false;
                        //isBelow = false;
                        //isAbove = false;
                        edgesToSplit.Add(edge);
                    }                  

                }
            }

            //if (!isOn && !isAbove && !isBelow)
            //{
            //    foreach (Edge edge in face.edges)
            //    {
                    
            //        if (edgeSides.TryGetValue(edge, out SplitSide side))
            //        {
            //            if (side == SplitSide.on) edgesToSplit.Add(edge);
            //        }
            //    }
            //}

        }
        mesh.Connect(edgesToSplit);
    }

    private void UpdateEdgeClassification(ProBuilderMesh mesh, Edge edge)
    {
        if (!edgeSides.ContainsKey(edge))
        {
            var vertices = mesh.GetVertices(new List<int> { edge.a, edge.b });
            if (vertices.Length == 2)
            {
                Vertex a = vertices[0];
                Vertex b = vertices[1];

                if ((vertexSides[a] == SplitSide.above && vertexSides[b] == SplitSide.below) || (vertexSides[a] == SplitSide.below && vertexSides[b] == SplitSide.above))
                {
                    edgeSides[edge] = SplitSide.intersect;
                }
                else if (vertexSides[a] == SplitSide.on && vertexSides[b] == SplitSide.on)
                {
                    edgeSides[edge] = SplitSide.on;
                }
                else if (vertexSides[a] == SplitSide.above && vertexSides[b] == SplitSide.above)
                {
                    edgeSides[edge] = SplitSide.above;
                }
                else
                {
                    edgeSides[edge] = SplitSide.below;
                }
            }
        }
    }

    bool isPointOnPlane(Plane plane, Vector3 point)
    {
        return Mathf.Abs(plane.GetDistanceToPoint(point)) < Mathf.Epsilon;
    }
    bool isPointAbovePlane(Plane plane, Vector3 point)
    {
        return plane.GetDistanceToPoint(point) > 0;
    }
    Vector3 DirectionNameToVecto3(SplitDirection dirName)
    {
        Vector3 dir = new Vector3(0, 0, 0);
        switch (dirName)
        {
            case SplitDirection.X:
                dir.x = 1;
                break;
            case SplitDirection.Y:
                dir.y = -1;
                break;
            case SplitDirection.Z:
                dir.z = 1;
                break;
            default:
                break;
        }
        return dir;
    }

    public enum SplitDirection
    {
        X,
        Y,
        Z
    }

    private enum SplitSide
    {
        below,
        above, 
        on,
        intersect
    }
}
