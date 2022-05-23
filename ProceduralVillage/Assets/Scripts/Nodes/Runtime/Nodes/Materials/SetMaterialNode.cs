using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

[System.Serializable, NodeMenuItem("Geometry/Materials/SetMaterial")]
public class SetMaterialNode : BaseFlowNode
{
    [Input("Input Material")]
    public Material InputMaterial;

    public override void Process(GraphFlow InputFlow)
    {
        if (InputFlow?.CurrentGameObject != null && InputMaterial != null)
        {
            ProBuilderMesh Mesh = InputFlow.CurrentGameObject.GetComponent<ProBuilderMesh>();
            if(Mesh != null)
            {
                Mesh.SetMaterial(Mesh.faces, InputMaterial);
                Mesh.ToMesh();
                Mesh.Refresh();
                OutputFlow = InputFlow;
            }

        }
    }

    }
