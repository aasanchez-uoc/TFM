using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable, NodeMenuItem("Geometry/Materials/SetMaterial")]
public class SetMaterialNode : GeometryFlowBaseNode
{
    [Input("Input Material")]
    public Material InputMaterial;

    [Output("Output Flow", allowMultiple = false)]
    public new GeometryFlow OutputFlow;

    protected override void Process(int index)
    {
        if (InputFlows == null || InputFlows.Count() == 0) return;
        GeometryFlow InputFlow = InputFlows.ToList()[index];
        if (InputFlow?.CurrentGameObject != null && InputMaterial != null)
        {
            InputFlow.Mesh.SetMaterial(InputFlow.Mesh.faces, InputMaterial);
            InputFlow.Mesh.ToMesh();
            InputFlow.Mesh.Refresh();
            OutputFlow = InputFlow;
        }
    }
}
