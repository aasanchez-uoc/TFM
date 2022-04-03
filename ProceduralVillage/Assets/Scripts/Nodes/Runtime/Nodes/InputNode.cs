using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputNode : ProceduralNode
{
    [Output("Start flow")]
    public GraphFlow StartFlow;

    protected override void Process()
    {
        if (StartFlow == null) StartFlow = new GraphFlow();
    }
}
 