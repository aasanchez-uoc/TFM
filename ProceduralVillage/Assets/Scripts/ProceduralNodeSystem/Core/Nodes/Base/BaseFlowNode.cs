using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Clase base para todos aquellos nodos que participan en el flujo de generación
/// </summary>
public abstract class BaseFlowNode : ProceduralNode
{
    #region Puertos
    [Output("Output flow", allowMultiple = false)]
    public GraphFlow OutputFlow;

    [Input("Input flows", allowMultiple =true)]
    public IEnumerable<GraphFlow> InputFlows;
    #endregion

    #region Atributos privados
    private List<List<GraphFlow>> flows;
    #endregion

    #region Métodos públicos
    public void OnProcess(int inputIndex, int subIndex = 0)
	{
		inputPorts.PullDatas();
		ExceptionToLog.Call(() => Process(inputIndex, subIndex));
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
            if (e.passThroughBuffer is IEnumerable<GraphFlow> flowEnum)
            {
                var flowList = flowEnum.ToList();
                flows.Add(flowList);
                list.AddRange(flowList);
            }

            if(e.passThroughBuffer is null && e.outputPort.fieldInfo.FieldType == typeof(GraphFlow))
            {
                flows.Add(new List<GraphFlow>());
            }
        }
        InputFlows = list;
    }
    #endregion

    #region Métodos privados
    protected sealed override void Process() => throw new Exception("Do not use");

	protected virtual void Process(int inputIndex, int subIndex)
    {
        if (flows == null || flows.Count <= inputIndex) return;
        List<GraphFlow> list = flows[inputIndex];
        if (list == null || list.Count() <= subIndex) return;
        GraphFlow InputFlow = list[subIndex];
        Process(InputFlow);  
	}

    public virtual int CountInputsOnEdge(int index)
    {
        if (flows == null || flows.Count <= index) return 0;
        return flows[index].Count;
    }

    public abstract void Process(GraphFlow inputFlow);
    #endregion
}
