using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Multiplica dos floats entre sí
/// </summary>
[System.Serializable, NodeMenuItem("Float/Multiplication")]
public class FloatMulNode : ProceduralNode
{

	[Input("A"), ShowAsDrawer]
	public float a;

	[Input("B"), ShowAsDrawer]
	public float b;

	[Output("Out")]
	public float o;

	public override string name => "Multiplication";

	protected override void Process()
	{
		o = a * b;
	}
}
