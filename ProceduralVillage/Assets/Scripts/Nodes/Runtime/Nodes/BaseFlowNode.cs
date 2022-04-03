using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFlowNode : ProceduralNode
{
    [Output("Output flow")]
    public GraphFlow OutputFlow;

    [Input("Input flow")]
    public GraphFlow InputFlow;
}
