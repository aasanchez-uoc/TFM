using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, NodeMenuItem("Constants/Float")]
public class FloatNode : ProceduralNode
{
	[Output(name = "Float")]
	public float Float = 1.0f;

	public override string name => "Float";
}
