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
    public int WidthCuts = 1;

    [Input, ShowAsDrawer]
    public int HeightCuts = 1;

    [Output("Output flow", allowMultiple = false)]
    public new GeometryFlow OutputFlow;


    [CustomPortInput(nameof(InputFlows), typeof(GraphFlow), allowCast = true)]
    public void GetInputs(List<SerializableEdge> edges)
    {
        InputFlows = edges.Select(e => (GraphFlow)e.passThroughBuffer);
    }

    protected override void Process(int index)
    {
        if (InputFlows == null || InputFlows.Count() == 0) return;
        GraphFlow InputFlow = InputFlows.ToList()[index];
        if (InputFlow?.CurrentGameObject != null)
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
