using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

//[System.Serializable, NodeMenuItem("Geometry/Mesh from points")]
//public class MeshFromPointsNode : GeometryFlowBaseNode
//{
//    [Input]
//    public List<Vector3> Points;

//    [Output("Output Flow", allowMultiple = false)]
//    public new GeometryFlow OutputFlow;

//    protected override void Process(int inputIndex)
//    {
//        if (InputFlows == null || InputFlows.Count() == 0) return;
//        GeometryFlow InputFlow = InputFlows.ToList()[inputIndex];
//        if (InputFlow?.CurrentGameObject != null && InputFlow.Mesh != null)
//        {

//            int[] indexes = new int[Points.Count * 3];

//            for (int i = 0; i < Points.Count - 1; i++)
//            {
//                indexes[i * 3] = 0;
//                indexes[i * 3 + 1] = Points.Count - i - 1;
//                indexes[i * 3 + 2] = Points.Count - i - 2 == 0 ? Points.Count - 1 : Points.Count - i - 2;
//            }

//            ProBuilderMesh quad = ProBuilderMesh.Create(
//                Points,
//                new Face[] { new Face(indexes) }
//            );
//            quad.Connect(quad.faces);
//            quad.ToMesh();

//            quad.Refresh(RefreshMask.All);
//            quad.gameObject.AddComponent<MeshCollider>();
//        }
//    }
//}
