using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Realiza un cast de int a float.
/// </summary>
[System.Serializable, NodeMenuItem("Float/IntToFloat")]
public class IntToFloat : ProceduralNode
{
	[Input("Input"), ShowAsDrawer]
	public int a;

	[Output("Out")]
	public float o;

	public override string name => "Float to Int";

	protected override void Process()
	{
		o = (float)a;
	}
}
