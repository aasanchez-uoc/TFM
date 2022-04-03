using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GeneratorGraph : ScriptableObject
{
    public List<NodeLink> NodeLinks = new List<NodeLink>();
    public List<BaseNodeData> Nodes = new List<BaseNodeData>();
    //public List<Property> ExposedProperties = new List<Property>();
    //public List<CommentBlockData> CommentBlockData = new List<CommentBlockData>();
}
