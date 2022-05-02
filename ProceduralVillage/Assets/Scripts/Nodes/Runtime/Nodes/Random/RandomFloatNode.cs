using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, NodeMenuItem("Random/Random Float Range")]
public class RandomFloatNode : ProceduralNode
{
	[Input("Min"), ShowAsDrawer]
	public float min;

	[Input("Max"), ShowAsDrawer]
	public float max;

	[Output("Out")]
	public float o;


	protected override void Process()
	{
		System.Random random = new System.Random();
		double value = random.NextDouble() * (max - min) + min;
		o = (float)(value);
	}
}
