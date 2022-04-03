using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class AssetCallbacks 

{
	[OnOpenAsset(0)]
	public static bool OnBaseGraphOpened(int instanceID, int line)
	{
		var asset = EditorUtility.InstanceIDToObject(instanceID);

		if (asset is ProceduralGraph)
		{
			ProceduralGraphWindow.Open(asset as ProceduralGraph);
			return true;
		}
		return false;
	}
}
