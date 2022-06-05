using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cambia el tamaño del GameObject modificando su Transform
/// </summary>
[System.Serializable, NodeMenuItem("Transform/Set Scale")]
public class SetScaleNode : BaseFlowNode
{

	[Input("Scale X"), ShowAsDrawer]
	public float x = 1;

	[Input("Scale Y"), ShowAsDrawer]
	public float y = 1;

	[Input("Scale Z"), ShowAsDrawer]
	public float z = 1;

    public override void Process(GraphFlow inputFlow)
    {
		if (inputFlow?.CurrentGameObject != null)
		{
			inputFlow.CurrentGameObject.transform.localScale = new Vector3(x, y, z);
			OutputFlow = inputFlow;
		}
	}
}
