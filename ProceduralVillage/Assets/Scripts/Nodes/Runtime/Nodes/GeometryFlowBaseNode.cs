using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GeometryFlowBaseNode : BaseFlowNode
{
    [Input("Input flows", allowMultiple = true)]
    public new IEnumerable<GeometryFlow> InputFlows;


    [CustomPortInput(nameof(InputFlows), typeof(GraphFlow), allowCast = true)]
    public void GetInputs(List<SerializableEdge> edges)
    {
        InputFlows = edges.Select(e => (GeometryFlow)e.passThroughBuffer);
    }
}
