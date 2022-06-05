using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Crea un material standard de Unity. Permite especificar el color y los parámetros Metallic y Smoothness.
/// </summary>
[System.Serializable, NodeMenuItem("Materials/StandardMaterial")]
public class StandardMaterialNode : ProceduralNode
{
    [Input, ShowAsDrawer]
    public Color MaterialColor;

    [Input, ShowAsDrawer]
    public float Metallic = 0.0f;

    [Input, ShowAsDrawer]
    public float Smoothness = 0.0f;

    public Texture2D Texture = null;

    [Output("Output Material")]
    public Material OutputMaterial;

    protected override void Process()
    {
        OutputMaterial = new Material(Shader.Find("Standard"));
        OutputMaterial.SetFloat("_Metallic", Metallic);
        OutputMaterial.SetFloat("_Glossiness", Smoothness);
        if(Texture != null) OutputMaterial.SetTexture("_MainTex", Texture);
        OutputMaterial.color = MaterialColor;
        
    }
}
