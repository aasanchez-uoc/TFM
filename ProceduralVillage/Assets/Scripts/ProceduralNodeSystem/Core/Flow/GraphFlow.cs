using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphFlow 
{
    public GameObject CurrentGameObject;

    public GraphFlow(string name  = "", bool init = true)
    {
        if(init)
            CurrentGameObject = (string.IsNullOrEmpty(name)) ? new GameObject() : new GameObject(name);
    }

    public GraphFlow() : this("", false)
    {
        
    }

    public GraphFlow(string Name, GameObject go, Transform parent)
    {
        CurrentGameObject = go;
        CurrentGameObject.transform.parent = parent;
        CurrentGameObject.transform.localPosition = new Vector3(0, 0, 0);
        CurrentGameObject.transform.localScale = new Vector3(1, 1, 1);
        CurrentGameObject.name = Name;
    }
}
