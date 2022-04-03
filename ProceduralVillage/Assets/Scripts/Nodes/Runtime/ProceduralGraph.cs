using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Procedural Graph", menuName = "Procedural/ProceduralGraph", order = 1)]
public class ProceduralGraph : BaseGraph
{
    public ProceduralGraph()
    {
        base.onEnabled += Enabled;
    }

    [System.NonSerialized]
    OutputNode _outputNode;

    [System.NonSerialized]
    InputNode _inputNode;

    public OutputNode outputNode
    {
        get
        {
            if (_outputNode == null)
                _outputNode = nodes.FirstOrDefault(n => n is OutputNode) as OutputNode;

            return _outputNode;
        }
        set => _outputNode = value;
    }


    public InputNode inputNode
    {
        get
        {
            if (_inputNode == null)
                _inputNode = nodes.FirstOrDefault(n => n is InputNode) as InputNode;

            return _inputNode;
        }
        set => _inputNode = value;
    }


    void Enabled()
    {
        
        if (outputNode == null)
            outputNode = AddNode(BaseNode.CreateFromType<OutputNode>(new Vector2(400, 100))) as OutputNode;
        if (inputNode == null)
            inputNode = AddNode(BaseNode.CreateFromType<InputNode>(new Vector2(0, 100))) as InputNode;

    }



}
