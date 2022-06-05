using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Genera una lista de puntos aleatorios dentro de unos límites especificados.
/// </summary>
[System.Serializable, NodeMenuItem("Random/Random Points")]
public class RandomPointsNode : ProceduralNode
{
	[Input("Number of points"), ShowAsDrawer]
	public int  number;

	[Input("Min X"), ShowAsDrawer]
	public float minX;

	[Input("Max X"), ShowAsDrawer]
	public float maxX;


	[Input("Min Z"), ShowAsDrawer]
	public float minZ;

	[Input("Max Z"), ShowAsDrawer]
	public float maxZ;

	[Output("Output points")]
	public List<Vector3> o;


	protected override void Process()
	{
		o = new List<Vector3>();

		for(int i = 0; i < number; i++)
        {
			Vector3 point = GetRandomPoint();
			o.Add(point);
		}


    }

    public Vector3 GetRandomPoint()
    {
        var target = new Vector3(
            UnityEngine.Random.Range(minX, maxX),
            0,
            UnityEngine.Random.Range(minZ, maxZ)
        );

        return target;
    }
}
