using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, NodeMenuItem("Flow/Repeat Flow")]
public class  RepeatFlowNode : BaseFlowNode
{

    [Input, ShowAsDrawer]
    public int Count = 1;

    [Output("Output Flow", allowMultiple = false)]
    public new List<GraphFlow> OutputFlow;

    public string ObjectsNames = "Repeat Block";

    public override void Process(GraphFlow inputFlow)
    {
        OutputFlow = new List<GraphFlow>();
        //OutputFlow.Add(inputFlow);
        GraphFlow previousFlow = inputFlow;

        for(int i = 0; i < Count; i++)
        {
            GraphFlow subFlow = new GraphFlow(ObjectsNames + " " + (i+1), true);
            subFlow.CurrentGameObject.transform.parent = previousFlow.CurrentGameObject.transform;
            subFlow.CurrentGameObject.transform.localPosition = new Vector3(0, 0, 0);
            subFlow.CurrentGameObject.transform.localScale = new Vector3(1, 1, 1);
            OutputFlow.Add(subFlow);
            previousFlow = subFlow;
        }
    }

}
