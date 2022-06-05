using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dada una lista de colores, escoge uno de ellos de forma aleatoria.
/// </summary>
[System.Serializable, NodeMenuItem("Random/Random Color")]
public class RandomColor : ProceduralNode
{
    [Input, ShowAsDrawer]
    public List<Color> InputColors;

    [Output("Out")]
    public Color o;

	System.Random random = new System.Random();
	protected override void Process()
	{
		if(InputColors != null && InputColors.Count > 0)
        {
            int i = random.Next(0, InputColors.Count - 1);
            o = InputColors[i];
        }

	}
}
