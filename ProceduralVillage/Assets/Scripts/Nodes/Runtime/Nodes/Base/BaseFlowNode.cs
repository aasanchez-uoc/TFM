using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseFlowNode : ProceduralNode
{
    [Output("Output flow", allowMultiple = false)]
    public GraphFlow OutputFlow;

    [Input("Input flows", allowMultiple =true)]
    public IEnumerable<GraphFlow> InputFlows;

	private List<List<GraphFlow>> flows;

	public void OnProcess(int inputIndex)
	{
		inputPorts.PullDatas();

		ExceptionToLog.Call(() => Process(inputIndex));

		InvokeOnProcessed();

		outputPorts.PushDatas();
	}

	[CustomPortInput(nameof(InputFlows), typeof(GraphFlow), allowCast = true)]
	public  virtual void GetInputs(List<SerializableEdge> edges)
	{
        flows = new List<List<GraphFlow>>();
        var list = new List<GraphFlow>();

        foreach (SerializableEdge e in edges)
        {
            if (e.passThroughBuffer is GraphFlow flow)
            {
                list.Add(flow);
                flows.Add(new List<GraphFlow>() { flow });
            }
            if (e.passThroughBuffer is List<GraphFlow> flowList)
            {
                flows.Add(flowList);
                list.AddRange(flowList);
            }
        }
        InputFlows = list;
    }

    protected sealed override void Process() => throw new Exception("Do not use");

	public virtual void Process(int inputIndex, int subIndex = 0)
    {
        if (flows == null || flows.Count <= inputIndex) return;
        List<GraphFlow> list = flows[inputIndex];
        if (list == null || list.Count() <= subIndex) return;
        GraphFlow InputFlow = list[subIndex];
        Process(InputFlow);  
	}

    public virtual int CountInputsOnEdge(int index)
    {
        if (flows == null) return 0;
        return flows[index].Count;
    }

    public abstract void Process(GraphFlow inputFlow);
}
