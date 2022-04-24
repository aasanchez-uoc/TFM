using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Geometry/Transformations/SplitFace")]
public class SplitFace : GeometryFlowBaseNode
{
    public Selector FaceSelector;

    public SpawnLevel NewObjectParent;

    [Output("Selected Faces", allowMultiple = false)]
    public new GeometryFlow OutputFlow;

    [Output("Remaining Faces", allowMultiple = false)]
    public GeometryFlow OtherOutputFlow;

    protected override void Process(int index)
    {
        if (InputFlows == null || InputFlows.Count() ==  0) return;
        List<Face> selectedFaces = new List<Face>();
        GeometryFlow InputFlow = InputFlows.ToList()[index];
        if (InputFlow?.CurrentGameObject != null && InputFlow.Mesh != null)
        {
            foreach (Face face in InputFlow.Mesh.faces)
            {
                bool isMatch = false;
                var normal = Math.Normal(InputFlow.Mesh, face);
                if (FaceSelector == Selector.Horizontal)
                {
                    if (Mathf.Abs(Vector3.Dot(normal, Vector3.up) ) >= System.Math.Cos(11.25 / 180.0 * System.Math.PI))
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

                if(isMatch)
                {
                    selectedFaces.Add(face);
                }
            }

            List<Face> detachedFaces = InputFlow.Mesh.DetachFaces(selectedFaces);

            if(OutputFlow != null)
            {
                UnityEngine.Object.DestroyImmediate(OutputFlow.CurrentGameObject);
            }


            OutputFlow = new GeometryFlow();
            OutputFlow.Mesh = ProBuilderMesh.Create(InputFlow.Mesh.positions, detachedFaces);

            if (NewObjectParent == SpawnLevel.Child)
            {
                OutputFlow.CurrentGameObject = OutputFlow.Mesh.gameObject;
            }
            else
            {
                OutputFlow.CurrentGameObject = InputFlow.CurrentGameObject;
            }

            OutputFlow.Mesh.transform.parent = InputFlow.CurrentGameObject.transform;


            OtherOutputFlow = InputFlow;

            OutputFlow.Mesh.ToMesh();
            OutputFlow.Mesh.Refresh();

            OtherOutputFlow.Mesh.ToMesh();
            OtherOutputFlow.Mesh.Refresh();
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