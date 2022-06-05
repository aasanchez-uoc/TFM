using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, NodeMenuItem("Materials/Color Node")]
public class ColorNode : ProceduralNode
{

	public Color Color;

	[Output("Out")]
	public Color o;

	protected override void Process()
	{
		o = Color;
	}
}
