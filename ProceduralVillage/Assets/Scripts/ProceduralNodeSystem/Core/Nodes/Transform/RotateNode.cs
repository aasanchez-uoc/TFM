using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rota el GameObject modificando su Transform
/// </summary>
[System.Serializable, NodeMenuItem("Transform/Rotate")]
public class RotateNode : BaseFlowNode
{
	[Input("Rotate X"), ShowAsDrawer]
	public float x = 0;

	[Input("Rotate Y"), ShowAsDrawer]
	public float y = 0;

	[Input("Rotate Z"), ShowAsDrawer]
	public float z = 0;

    public override void Process(GraphFlow inputFlow)
    {
		if (inputFlow?.CurrentGameObject != null)
		{
			inputFlow.CurrentGameObject.transform.Rotate(new Vector3(x, y, z));
			OutputFlow = inputFlow;
		}
	}
}
