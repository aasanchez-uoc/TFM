using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Realiza un cast de float a int
/// </summary>
[System.Serializable, NodeMenuItem("Float/ToInt")]
public class FloatToInt : ProceduralNode
{
	[Input("Input"), ShowAsDrawer]
	public float a;

	public RoundType RoundMode = RoundType.Nearest;

	[Output("Out")]
	public int o;

	public override string name => "Float to Int";

	protected override void Process()
	{
        switch (RoundMode)
        {
            case RoundType.Nearest:
                o = (int)Math.Round(a, 0);
                break;
            case RoundType.Up:
                o = (int)Math.Ceiling(a);
                break;
            case RoundType.Down:
                o = (int)a;
                break;
            default:
                break;
        }
        
	}

	public enum RoundType
    {
		Nearest,
		Up,
		Down
    }
}
