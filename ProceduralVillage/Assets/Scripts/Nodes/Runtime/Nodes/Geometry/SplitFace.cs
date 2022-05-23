using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Geometry/Transformations/SplitFace")]
public class SplitFace : BaseFlowNode
{
    public Selector FaceSelector;

    public SpawnLevel NewObjectParent;

    [Output("Remaining Faces", allowMultiple = false)]
    public GraphFlow OtherOutputFlow;

    public string ObjectName = "Split Face";

    public override void Process(GraphFlow InputFlow)
    {
        if (InputFlow?.CurrentGameObject != null)
        {
            ProBuilderMesh Mesh = InputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>();
            if (Mesh != null)
            {
                List<Face> selectedFaces = new List<Face>();
                foreach (Face face in Mesh.faces)
                {
                    bool isMatch = false;
                    var normal = Math.Normal(Mesh, face);
                    if (FaceSelector == Selector.Horizontal)
                    {
                        if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) >= System.Math.Cos(11.25 / 180.0 * System.Math.PI))
                        {
                            isMatch = true;
                        }
                    }
                    else if (FaceSelector == Selector.Vertical)
                    {
                        if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < System.Math.Cos(78.75 / 180.0 * System.Math.PI))
                        {
                            isMatch = true;
                        }
                    }
                    else if (FaceSelector == Selector.Left)
                    {
                        if (normal.x < 0.0 && Mathf.Abs(normal.x) >= Mathf.Abs(normal.y) &&
                            Mathf.Abs(normal.x) > Mathf.Abs(normal.z))
                        {
                            isMatch = true;
                        }
                    }
                    else if (FaceSelector == Selector.Right)
                    {
                        if (normal.x > 0.0 && Mathf.Abs(normal.x) >= Mathf.Abs(normal.y) &&
                            Mathf.Abs(normal.x) > Mathf.Abs(normal.z))
                        {
                            isMatch = true;
                        }
                    }
                    else if (FaceSelector == Selector.Bottom)
                    {
                        if (normal.y < 0.0 && Mathf.Abs(normal.y) >= Mathf.Abs(normal.z) &&
                            Mathf.Abs(normal.y) > Mathf.Abs(normal.x))
                        {
                            isMatch = true;
                        }
                    }
                    else if (FaceSelector == Selector.Top)
                    {
                        if (normal.y > 0.0 && Mathf.Abs(normal.y) >= Mathf.Abs(normal.z) &&
                            Mathf.Abs(normal.y) > Mathf.Abs(normal.x))
                        {
                            isMatch = true;
                        }
                    }
                    else if (FaceSelector == Selector.Back)
                    {
                        if (normal.z < 0.0 && Mathf.Abs(normal.z) >= Mathf.Abs(normal.x) &&
                            Mathf.Abs(normal.z) > Mathf.Abs(normal.y))
                        {
                            isMatch = true;
                        }
                    }
                    else if (FaceSelector == Selector.Front)
                    {
                        if (normal.z > 0.0 && Mathf.Abs(normal.z) >= Mathf.Abs(normal.x) &&
                            Mathf.Abs(normal.z) > Mathf.Abs(normal.y))
                        {
                            isMatch = true;
                        }
                    }

                    if (isMatch)
                    {
                        selectedFaces.Add(face);
                    }
                }

                List<Face> detachedFaces = Mesh.DetachFaces(selectedFaces);
                ProBuilderMesh newMesh = ProBuilderMesh.Create(Mesh.positions, detachedFaces);
                OutputFlow = new GraphFlow(ObjectName, newMesh.gameObject, InputFlow.CurrentGameObject.transform);

                newMesh.ToMesh();
                newMesh.Refresh();

                Mesh.ToMesh();
                Mesh.Refresh();

                OtherOutputFlow = InputFlow;
            }
        }
    }

    }



public enum Selector
{
    Front,
    Back,
    Left,
    Right,
    Top,
    Bottom,
    Horizontal, 
    Vertical
}

public enum SpawnLevel
{
    Child,
    SameLevel
}