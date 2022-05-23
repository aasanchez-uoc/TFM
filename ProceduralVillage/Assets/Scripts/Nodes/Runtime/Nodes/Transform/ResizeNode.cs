using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, NodeMenuItem("Transform/Resize")]
public class ResizeNode : BaseFlowNode
{

	[Input("Scale X"), ShowAsDrawer]
	public float x = 0;

	[Input("Scale Y"), ShowAsDrawer]
	public float y = 0;

	[Input("Scale Z"), ShowAsDrawer]
	public float z = 0;

    public override void Process(GraphFlow inputFlow)
    {
		if (inputFlow?.CurrentGameObject != null)
		{
			inputFlow.CurrentGameObject.transform.localScale =  Vector3.Scale(inputFlow.CurrentGameObject.transform.localScale, new Vector3(x , y, z));
			OutputFlow = inputFlow;
		}
	}
}
