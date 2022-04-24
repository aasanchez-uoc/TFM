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


	public void OnProcess(int inputIndex)
	{
		inputPorts.PullDatas();

		ExceptionToLog.Call(() => Process(inputIndex));

		InvokeOnProcessed();

		outputPorts.PushDatas();
	}

	protected sealed override void Process() => throw new Exception("Do not use");

	protected abstract void Process(int inputIndex);
}
