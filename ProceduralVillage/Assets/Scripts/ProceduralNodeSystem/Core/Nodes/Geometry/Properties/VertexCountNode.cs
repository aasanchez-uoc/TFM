using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

/// <summary>
/// Calcula el número de vértices del Mesh. Si no existe Mesh, devuelve 0.
/// </summary>
[System.Serializable, NodeMenuItem("Geometry/Properties/Get Vertex Count")]
public class VertexCountNode : BaseFlowNode
{
    [Output("Vertex Count")]
    public int VertexCount;
    public override void Process(GraphFlow inputFlow)
    {
        OutputFlow = null;
        VertexCount = 0;
        if (inputFlow?.CurrentGameObject?.GetComponent<ProBuilderMesh>() != null)
        {
            ProBuilderMesh mesh = inputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>();
            VertexCount = mesh.vertexCount;
            OutputFlow = inputFlow;
        }

    }
}
