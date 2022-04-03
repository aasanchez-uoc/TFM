using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;


[System.Serializable, NodeMenuItem("Geometry/Primitives/Plane")]
public class PlaneNode : BaseFlowNode
{
    [Input, ShowAsDrawer]
    public float Width = 1;

    [Input, ShowAsDrawer]
    public float Height = 1;

    [Input, ShowAsDrawer]
    public int WidthCuts = 1;

    [Input, ShowAsDrawer]
    public int HeightCuts = 1;

    [Output("Output flow")]
    public new GeometryFlow OutputFlow;

    protected override void Process()
    {
        if(InputFlow?.CurrentGameObject != null)
        {
            ProBuilderMesh m_Mesh = ShapeGenerator.GeneratePlane(PivotLocation.Center, Width, Height, WidthCuts, HeightCuts, Axis.Up);
            m_Mesh.SetMaterial(m_Mesh.faces, BuiltinMaterials.defaultMaterial);
            m_Mesh.transform.parent = InputFlow.CurrentGameObject.transform;
            OutputFlow = new GeometryFlow();
            OutputFlow.CurrentGameObject = InputFlow.CurrentGameObject;
            OutputFlow.Mesh = m_Mesh;
        }
    }
}
