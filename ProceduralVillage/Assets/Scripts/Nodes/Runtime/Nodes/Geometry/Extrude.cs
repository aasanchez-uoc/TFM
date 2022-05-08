using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Geometry/Transformations/Extrude")]
public class Extrude : GeometryFlowBaseNode
{

    [Input, ShowAsDrawer]
    public float Height = 1;

    [Output("Output flow", allowMultiple = false)]
    public new GeometryFlow OutputFlow;

    [Output("Extruded faces", allowMultiple = false)]
    public Face[] ExtrudedFaces;

    public ExtrudeMethod extrudeMethod = ExtrudeMethod.IndividualFaces;


    public override void Process(GraphFlow inputflow)
    {
        GeometryFlow InputFlow = (GeometryFlow) inputflow ;
        if (InputFlow?.CurrentGameObject != null)
        {
            ExtrudedFaces = ExtrudeElements.Extrude(InputFlow.Mesh, InputFlow.Mesh.faces, extrudeMethod, Height);
            InputFlow.Mesh.ToMesh();
            InputFlow.Mesh.Refresh();
            OutputFlow = InputFlow;
        }
    }
}
