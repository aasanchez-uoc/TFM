using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Divide el flujo de entrada en dos, creando un nuevo GameObject.
/// </summary>
[System.Serializable, NodeMenuItem("Flow/Diverge Flow")]
public class DivergeFlowNode : BaseFlowNode
{
    [Output("New flow", allowMultiple = false)]
    public GraphFlow OtherOutputFlow;

    public override void Process(GraphFlow inputFlow)
    {
        OutputFlow = inputFlow;
        OtherOutputFlow = new GraphFlow("New Flow", true);
        OtherOutputFlow.CurrentGameObject.transform.parent = inputFlow.CurrentGameObject.transform;
    }
}
