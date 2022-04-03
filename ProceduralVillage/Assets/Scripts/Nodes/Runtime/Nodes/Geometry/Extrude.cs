using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Geometry/Transformations/Extrude")]
public class Extrude : BaseFlowNode
{

    [Input, ShowAsDrawer]
    public float Height = 1;

    [Input("Input flow")]
    public new GeometryFlow InputFlow;

    [Output("Output flow")]
    public new GeometryFlow OutputFlow;

    [Output("Extruded faces")]
    public Face[] ExtrudedFaces;

    public ExtrudeMethod extrudeMethod = ExtrudeMethod.IndividualFaces;
    protected override void Process()
    {
        if (InputFlow?.CurrentGameObject != null)
        {
            ExtrudedFaces = ExtrudeElements.Extrude(InputFlow.Mesh, InputFlow.Mesh.faces, extrudeMethod, Height);
            InputFlow.Mesh.ToMesh();
            InputFlow.Mesh.Refresh();
            OutputFlow = InputFlow;
        }
    }
}
