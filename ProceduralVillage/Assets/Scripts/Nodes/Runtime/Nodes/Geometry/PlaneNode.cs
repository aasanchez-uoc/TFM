using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public int WidthCuts = 0;

    [Input, ShowAsDrawer]
    public int HeightCuts = 0;

    [Output("Output flow", allowMultiple = false)]
    public new GeometryFlow OutputFlow;



    public override void Process(GraphFlow inputFlow)
    {
        if (inputFlow?.CurrentGameObject != null)
        {
            ProBuilderMesh m_Mesh = ShapeGenerator.GeneratePlane(PivotLocation.Center, Width, Height, WidthCuts, HeightCuts, Axis.Up);
            m_Mesh.SetMaterial(m_Mesh.faces, BuiltinMaterials.defaultMaterial);
            m_Mesh.transform.parent = inputFlow.CurrentGameObject.transform;
            OutputFlow = new GeometryFlow();
            OutputFlow.CurrentGameObject = inputFlow.CurrentGameObject;
            OutputFlow.Mesh = m_Mesh;
        }
    }

 }
