using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Genera un float aleatorio dentro de un rango de valores especificados.
/// </summary>
[System.Serializable, NodeMenuItem("Random/Random Float Range")]
public class RandomFloatNode : ProceduralNode
{
	[Input("Min"), ShowAsDrawer]
	public float min;

	[Input("Max"), ShowAsDrawer]
	public float max;

	[Output("Out")]
	public float o;

	System.Random random = new System.Random();
	protected override void Process()
	{

		double value = random.NextDouble() * (max - min) + min;
		o = (float)(value);
	}
}
