using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable, NodeMenuItem("Math/Float Mul")]
public class FloatMulNode : ProceduralNode
{


	[Input("A"), ShowAsDrawer]
	public float a;

	[Input("B"), ShowAsDrawer]
	public float b;

	[Output("Out")]
	public float o;

	public override string name => "Mul";

	protected override void Process()
	{
		o = a * b;
	}
}