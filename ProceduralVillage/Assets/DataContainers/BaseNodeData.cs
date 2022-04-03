using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseNodeData
{

    public string Name;
    public Vector2 Position;
    public string GUID;
    public bool StartingPoint = false;

    public virtual void ProcessNode()
    {

    }
}
