using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// En función del valor del booleano Condition ejecuta un flujo u otro.
/// </summary>
[System.Serializable, NodeMenuItem("Flow/Diverge Flow")]
public class ConditionalFlowNode : BaseFlowNode
{
    [Output("False", allowMultiple = false)]
    public GraphFlow OtherOutputFlow;

    [Output("True", allowMultiple = false)]
    public new GraphFlow OutputFlow;

    public bool Condition;
    public override void Process(GraphFlow inputFlow)
    {
        OtherOutputFlow = null;
        OutputFlow = null;
        if (Condition) OutputFlow = inputFlow;
        else OtherOutputFlow = inputFlow;
    }
}
