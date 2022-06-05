using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cambia la posición del GameObject modificando su Transform
/// </summary>
[System.Serializable, NodeMenuItem("Transform/Traslate")]
public class TraslateNode : BaseFlowNode
{
	[Input("Traslate X"), ShowAsDrawer]
	public float x = 1;

	[Input("Traslate Y"), ShowAsDrawer]
	public float y = 1;

	[Input("Traslate Z"), ShowAsDrawer]
	public float z = 1;

    public override void Process(GraphFlow inputFlow)
    {
		if (inputFlow?.CurrentGameObject != null)
		{
			inputFlow.CurrentGameObject.transform.Translate(new Vector3(x, y, z));
			OutputFlow = inputFlow;
		}
	}
}
