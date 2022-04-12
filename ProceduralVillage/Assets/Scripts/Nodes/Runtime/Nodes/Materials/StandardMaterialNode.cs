using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, NodeMenuItem("Materials/StandardMaterial")]
public class StandardMaterialNode : ProceduralNode
{
    [Input, ShowAsDrawer]
    public Color MaterialColor;

    [Output("Output Material")]
    public Material OutputMaterial;

    protected override void Process()
    {
        OutputMaterial = new Material(Shader.Find("Standard"));
        OutputMaterial.color = MaterialColor;
        
    }
}
