using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Transform/Set Pivot Position")]
public class SetPivotPositionNode : BaseFlowNode
{
    [Input, ShowAsDrawer]
    public Vector3 LocalPosition = new Vector3(0,0,0);

    public override void Process(GraphFlow InputFlow)
    {
        if (InputFlow?.CurrentGameObject != null && InputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>() != null)
        {
            Vector3 worldPos = InputFlow.CurrentGameObject.transform.parent.TransformPoint(LocalPosition);
            InputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>().SetPivot(worldPos);
            OutputFlow = InputFlow;
        }
    }
}
