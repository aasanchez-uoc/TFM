using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable, NodeMenuItem("Geometry/Voronoi")]
public class VoronoiNode : BaseFlowNode
{
    [Input]
    public List<Vector3> Points;

    [Output("Output Flow", allowMultiple = false)]
    public new List<GeometryFlow> OutputFlow;


    public override void Process(GraphFlow InputFlow)
    {
        OutputFlow = new List<GeometryFlow>();
        if (InputFlow?.CurrentGameObject != null)
        {

            int[] indexes = new int[Points.Count * 3];
            for (int i = 0; i < Points.Count - 1; i++)
            {
                indexes[i * 3] = 0;
                indexes[i * 3 + 1] = Points.Count - i - 1;
                indexes[i * 3 + 2] = Points.Count - i - 2 == 0 ? Points.Count - 1 : Points.Count - i - 2;
            }

            //if (!TriangulationUtils.VoronoiFromVertices(Points.ToArray(), indexes.ToList(), out List<Face> faces, out List<Vector2> finalPoints)) return;
            if (!TriangulationUtils.VoronoiFacesFromPoints(Points.ToArray(), indexes.ToList(), out List<List<Vector2>> polyFaces)) return;


            foreach (List<Vector2> poly in polyFaces)
            {
                List<Vector3> points = new List<Vector3>();
                foreach (Vector2 point in poly)
                {
                    points.Add(new Vector3(point.x, 0, point.y));
                }
                ProBuilderMesh m_Mesh = ProBuilderMesh.Create();
                m_Mesh.CreateShapeFromPolygon(points, 0, false);
                m_Mesh.SetMaterial(m_Mesh.faces, BuiltinMaterials.defaultMaterial);
                m_Mesh.transform.parent = InputFlow.CurrentGameObject.transform;
                m_Mesh.ToMesh();
                m_Mesh.Refresh();

                GeometryFlow output = new GeometryFlow();
                output.CurrentGameObject = m_Mesh.gameObject;
                output.Mesh = m_Mesh;
                OutputFlow.Add(output);
            }
        }
    }

}
