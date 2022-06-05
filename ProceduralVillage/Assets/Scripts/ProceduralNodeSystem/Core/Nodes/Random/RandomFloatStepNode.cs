using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, NodeMenuItem("Random/Random Float Range Step")]
public class RandomFloatStepNode : ProceduralNode
{
	[Input("Min"), ShowAsDrawer]
	public float min = 0;

	[Input("Max"), ShowAsDrawer]
	public float max = 1;

	[Input("Step"), ShowAsDrawer]
	public float step = 0.1f;

	[Output("Out")]
	public float o;

	private System.Random random = new System.Random();
	protected override void Process()
	{
		int options = (int)System.Math.Ceiling((max - min) / step + 1);

		float value = random.Next(0, options) * step + min;
		o = (float)(Mathf.Clamp(value, min, max));
	}
}
