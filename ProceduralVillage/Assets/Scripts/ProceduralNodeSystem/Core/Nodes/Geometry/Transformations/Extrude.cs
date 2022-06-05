using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

/// <summary>
/// Realiza una operación de extrusión a la geometría que llegue por inputFlow.
/// </summary>
[System.Serializable, NodeMenuItem("Geometry/Transformations/Extrude")]
public class Extrude : BaseFlowNode
{
    [Input, ShowAsDrawer]
    public float Height = 1.0f;

    public ExtrudeMethod extrudeMethod = ExtrudeMethod.IndividualFaces;


    public override void Process(GraphFlow InputFlow)
    {
        if (InputFlow?.CurrentGameObject != null)
        {
            ProBuilderMesh Mesh = InputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>();
            if (Mesh != null)
            {
                var ExtrudedFaces = ExtrudeElements.Extrude(Mesh, Mesh.faces, extrudeMethod, Height);
                Mesh.ToMesh();
                Mesh.Refresh();
                OutputFlow = InputFlow;
            }
        }
    }
}
