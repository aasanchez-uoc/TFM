using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

/// <summary>
/// Calcula el área del Mesh. Si no existe Mesh, devuelve 0
/// </summary>
[System.Serializable, NodeMenuItem("Geometry/Properties/Get Mesh Area")]
public class MeshAreaNode : BaseFlowNode

{
    [Output("Mesh Area")]
    public float MeshArea;
    public override void Process(GraphFlow inputFlow)
    {
        OutputFlow = null;
        MeshArea = 0;
        if (inputFlow != null && inputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>() != null)
        {
            ProBuilderMesh mesh = inputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>();
            MeshArea = mesh.GetMeshArea(mesh.faces);
            OutputFlow = inputFlow;
        }

    }
}
