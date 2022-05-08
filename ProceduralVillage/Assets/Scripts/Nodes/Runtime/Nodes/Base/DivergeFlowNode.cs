using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, NodeMenuItem("Flow/Diverge Flow")]
public class DivergeFlowNode : BaseFlowNode
{
    [Output("New flow", allowMultiple = false)]
    public GraphFlow OtherOutputFlow;

    public override void Process(GraphFlow inputFlow)
    {
        OtherOutputFlow = new GraphFlow("New Flow", true);
        OtherOutputFlow.CurrentGameObject.transform.parent = inputFlow.CurrentGameObject.transform;
    }
}
