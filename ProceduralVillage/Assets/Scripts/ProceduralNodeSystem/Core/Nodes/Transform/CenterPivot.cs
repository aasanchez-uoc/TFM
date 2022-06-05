using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

/// <summary>
/// Centra el Pivot Point del Mesh
/// </summary>
[System.Serializable, NodeMenuItem("Transform/Center Pivot")]
public class SetCenterPivot : BaseFlowNode
{
    public override void Process(GraphFlow InputFlow)
    {
        if (InputFlow?.CurrentGameObject != null && InputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>() != null)
        {
            InputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>().CenterPivot(null);
            OutputFlow = InputFlow;
        }
    }
}
