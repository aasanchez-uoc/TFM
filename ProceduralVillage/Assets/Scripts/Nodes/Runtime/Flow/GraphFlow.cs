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
}
