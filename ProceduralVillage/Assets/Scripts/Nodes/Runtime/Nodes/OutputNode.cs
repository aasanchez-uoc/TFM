using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputNode :  ProceduralNode
{

    [Input("Input flow")]
    public GraphFlow inputFlow;
    public Texture PreviewTexture { get; private set; }


}
