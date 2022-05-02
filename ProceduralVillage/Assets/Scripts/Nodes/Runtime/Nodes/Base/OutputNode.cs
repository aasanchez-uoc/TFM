using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutputNode :  ProceduralNode
{

    [Input("Input flows", allowMultiple = true)]
    public IEnumerable<GraphFlow> InputFlows;
    public Texture PreviewTexture { get; private set; }


    [CustomPortInput(nameof(InputFlows), typeof(GraphFlow), allowCast = true)]
    public void GetInputs(List<SerializableEdge> edges)
    {
        InputFlows = edges.Select(e => (GraphFlow)e.passThroughBuffer);
    }
}
