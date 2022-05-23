//using GraphProcessor;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public abstract class GeometryFlowBaseNode : BaseFlowNode
//{
//    [Input("Input flows", allowMultiple = true)]
//    public new IEnumerable<GeometryFlow> InputFlows;

//    private List<List<GeometryFlow>> flows;

//    [CustomPortInput(nameof(InputFlows), typeof(GeometryFlow), allowCast = true)]
//    public override void GetInputs(List<SerializableEdge> edges)
//    {
//        flows = new List<List<GeometryFlow>>();
//        var list = new List<GeometryFlow>();

//        foreach(SerializableEdge e in edges)
//        {
//            if( e.passThroughBuffer is GeometryFlow flow)
//            {
//                list.Add(flow);
//                flows.Add(new List<GeometryFlow>() { flow });
//            }
//            if (e.passThroughBuffer is IEnumerable<GeometryFlow> flowEnum)
//            {
//                var flowList = flowEnum.ToList();
//                flows.Add(flowList);
//                list.AddRange(flowList);
//            }

//        }
//        InputFlows = list;
//    }

//    protected override void Process(int inputIndex, int subIndex = 0)
//    {
//        if (flows == null || flows.Count <= inputIndex)
//        { 
//            return; 
//        }
//        List<GeometryFlow> list = flows[inputIndex];
//        if (list == null || list.Count() <= subIndex)
//        {
//            return;
//        }
//        GraphFlow InputFlow = list[subIndex];
//        Process(InputFlow);
//    }
//    public override int CountInputsOnEdge(int index)
//    {
//        if (flows == null || flows.Count <= index) return 0;
//        return flows[index].Count;
//    }

//}
