using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

/// <summary>
/// Genera un cono, especificando su radio y altura.
/// </summary>
[System.Serializable, NodeMenuItem("Geometry/Primitives/Cone")]
public class ConeNode : BaseFlowNode
{
    [Input, ShowAsDrawer]
    public float Radius = 0.5f;

    [Input, ShowAsDrawer]
    public float Height = 1;

    [Input, ShowAsDrawer]
    public int AxisDivisions = 8;

    public PivotLocation PivotLocation = PivotLocation.Center;
    public string ObjectName = "Cone";


    public override void Process(GraphFlow inputFlow)
    {
        if (inputFlow?.CurrentGameObject != null)
        {
            ProBuilderMesh m_Mesh = ShapeGenerator.GenerateCone(PivotLocation, Radius, Height, AxisDivisions );
            m_Mesh.SetMaterial(m_Mesh.faces, BuiltinMaterials.defaultMaterial);
            m_Mesh.transform.parent = inputFlow.CurrentGameObject.transform;

            OutputFlow = new GraphFlow(ObjectName, m_Mesh.gameObject, inputFlow.CurrentGameObject.transform);

        }
    }

 }
