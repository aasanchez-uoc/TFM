using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

[System.Serializable, NodeMenuItem("Flow/Check Null Mesh")]
public class CheckNullMeshNode : BaseFlowNode
{
    [Output("Is Null", allowMultiple = false)]
    public GraphFlow OtherOutputFlow;

    public override void Process(GraphFlow inputFlow)
    {
        OtherOutputFlow = null;
        OutputFlow = null;
        if (inputFlow != null && inputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>() != null) 
        {
            OutputFlow = inputFlow;
        }
        else OtherOutputFlow = inputFlow;
    }
}
