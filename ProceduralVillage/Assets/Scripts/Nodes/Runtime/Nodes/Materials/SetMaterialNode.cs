using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, NodeMenuItem("Geometry/Materials/SetMaterial")]
public class SetMaterialNode : BaseFlowNode
{
    [Input("Input Flow")]
    public new GeometryFlow InputFlow;

    [Input("Input Material")]
    public Material InputMaterial;

    [Output("Output Flow")]
    public new GeometryFlow OutputFlow;

    protected override void Process()
    {
        if (InputFlow?.CurrentGameObject != null && InputMaterial != null)
        {
            InputFlow.Mesh.SetMaterial(InputFlow.Mesh.faces, InputMaterial);
            InputFlow.Mesh.ToMesh();
            InputFlow.Mesh.Refresh();
            OutputFlow = InputFlow;
        }
    }
}
