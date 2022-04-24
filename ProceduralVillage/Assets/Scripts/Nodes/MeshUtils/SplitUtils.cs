//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;


//namespace UnityEngine.ProBuilder.MeshOperations
//{
//    public static class SplitUtils
//    {
//        static bool ApplySplit(Face face, Vector3 dir, float size, List<float> sizes)
//        {

//            if (sizes == null || sizes.Count == 0)
//                return false;

//            float distance = 0.0f;
//            for (int i = 0; i < sizes.Count; ++i)
//            {
//                distance += sizes[i];
//                Face below, above;
//                if (i < (sizes.Count - 1) || distance < size - Mathf.Epsilon)
//                {
//                    //tmp->Split(geometry::Plane3(dir, distance), &below, &above);
//                    //tmp = above;
//                }
//                else
//                {
//                    //below = tmp;
//                }
//            }
//            return false;
//        }

//        static void Split(this ProBuilderMesh mesh, Face face, Plane plane, out Face below, out Face above)
//        {

//            IEnumerable<Edge> edges = face.edges;
//            Edge[] empty;
//            Face[] res;
//            Connect(mesh, edges, out res, out empty, face);

//            below = new Face();
//            above = new Face();

//        }


//        internal static ActionResult Connect(
//        this ProBuilderMesh mesh,
//        IEnumerable<Edge> edges,
//        out Face[] addedFaces,
//        out Edge[] connections,
//        Face SelectedFace)
//        {

//            List<WingedEdge> wings = WingedEdge.GetWingedEdges(mesh);

//            // map each edge to a face so that we have a list of all touched faces with their to-be-subdivided edges
//            Dictionary<Face, List<WingedEdge>> touched = new Dictionary<Face, List<WingedEdge>>();

//            foreach (WingedEdge wing in wings)
//            {
//                if (edges.Contains(wing.edge))
//                {
//                    List<WingedEdge> faceEdges;
//                    if (touched.TryGetValue(wing.face, out faceEdges))
//                        faceEdges.Add(wing);
//                    else
//                        touched.Add(wing.face, new List<WingedEdge>() { wing });
//                }
//            }

//            Dictionary<Face, List<WingedEdge>> affected = new Dictionary<Face, List<WingedEdge>>();

//            // weed out edges that won't actually connect to other edges (if you don't play ya' can't stay)
//            foreach (KeyValuePair<Face, List<WingedEdge>> kvp in touched)
//            {
//                if (kvp.Value.Count <= 1)
//                {
//                    WingedEdge opp = kvp.Value[0].opposite;

//                    if (opp == null)
//                        continue;

//                    List<WingedEdge> opp_list;

//                    if (!touched.TryGetValue(opp.face, out opp_list))
//                        continue;

//                    if (opp_list.Count <= 1)
//                        continue;
//                }

//                affected.Add(kvp.Key, kvp.Value);
//            }

//            List<Vertex> vertices = new List<Vertex>(mesh.GetVertices());
//            List<ConnectFaceRebuildData> results = new List<ConnectFaceRebuildData>();
//            // just the faces that where connected with > 1 edge
//            List<Face> connectedFaces = new List<Face>();

//            HashSet<int> usedTextureGroups = new HashSet<int>(mesh.faces.Select(x => x.textureGroup));
//            int newTextureGroupIndex = 1;

//            // do the splits
//            foreach (KeyValuePair<Face, List<WingedEdge>> split in affected)
//            {
//                Face face = split.Key;
//                List<WingedEdge> targetEdges = split.Value;
//                int inserts = targetEdges.Count;
//                Vector3 nrm = NormalFromVertices(vertices, face.indexes);

//                List<ConnectFaceRebuildData> res = inserts == 2 ?
//                    ConnectEdgesInFace(face, targetEdges[0], targetEdges[1], vertices) :
//                    ConnectEdgesInFace(face, targetEdges, vertices);

//                if (face.textureGroup < 0)
//                {
//                    while (usedTextureGroups.Contains(newTextureGroupIndex))
//                        newTextureGroupIndex++;

//                    usedTextureGroups.Add(newTextureGroupIndex);
//                }


//                if (res == null)
//                {
//                    connections = null;
//                    addedFaces = null;
//                    return new ActionResult(ActionResult.Status.Failure, "Unable to connect faces");
//                }
//                else
//                {
//                    foreach (ConnectFaceRebuildData c in res)
//                    {
//                        connectedFaces.Add(c.faceRebuildData.face);

//                        Vector3 fn = Math.Normal(c.faceRebuildData.vertices,
//                            c.faceRebuildData.face.indexesInternal);

//                        if (Vector3.Dot(nrm, fn) < 0)
//                            c.faceRebuildData.face.Reverse();

//                        c.faceRebuildData.face.textureGroup =
//                            face.textureGroup < 0 ? newTextureGroupIndex : face.textureGroup;
//                        c.faceRebuildData.face.uv = new AutoUnwrapSettings(face.uv);
//                        c.faceRebuildData.face.submeshIndex = face.submeshIndex;
//                        c.faceRebuildData.face.smoothingGroup = face.smoothingGroup;
//                        c.faceRebuildData.face.manualUV = face.manualUV;
//                    }

//                    results.AddRange(res);
//                }
                

//            }

//            FaceRebuildData.Apply(results.Select(x => x.faceRebuildData), mesh, vertices, null);

//            mesh.sharedTextures = new SharedVertex[0];
//            int removedVertexCount = mesh.DeleteFaces(affected.Keys).Length;
//            mesh.sharedVertices = SharedVertex.GetSharedVerticesWithPositions(mesh.positionsInternal);
//            mesh.ToMesh();

//            // figure out where the new edges where inserted
//            if (returnEdges)
//            {
//                // offset the newVertexIndexes by whatever the FaceRebuildData did so we can search for the new edges by index
//                var appended = new HashSet<int>();

//                for (int n = 0; n < results.Count; n++)
//                    for (int i = 0; i < results[n].newVertexIndexes.Count; i++)
//                        appended.Add((results[n].newVertexIndexes[i] + results[n].faceRebuildData.Offset()) - removedVertexCount);

//                Dictionary<int, int> lup = mesh.sharedVertexLookup;
//                IEnumerable<Edge> newEdges = results.SelectMany(x => x.faceRebuildData.face.edgesInternal).Where(x => appended.Contains(x.a) && appended.Contains(x.b));
//                IEnumerable<EdgeLookup> distNewEdges = EdgeLookup.GetEdgeLookup(newEdges, lup);

//                connections = distNewEdges.Distinct().Select(x => x.local).ToArray();
//            }
//            else
//            {
//                connections = null;
//            }

//            if (returnFaces)
//                addedFaces = connectedFaces.ToArray();
//            else
//                addedFaces = null;

//            return new ActionResult(ActionResult.Status.Success, string.Format("Connected {0} Edges", results.Count / 2));
//        }
//        internal static Vector3 NormalFromVertices(IList<Vertex> vertices, IList<int> indexes = null)
//        {
//            if (indexes == null || indexes.Count % 3 != 0)
//            {
//                Vector3 cross = Vector3.Cross(vertices[1].position - vertices[0].position, vertices[2].position - vertices[0].position);
//                cross.Normalize();
//                return cross;
//            }
//            else
//            {
//                int len = indexes.Count;
//                Vector3 nrm = Vector3.zero;

//                for (int i = 0; i < len; i += 3)
//                    nrm += Math.Normal(vertices[indexes[i]].position, vertices[indexes[i + 1]].position, vertices[indexes[i + 2]].position);

//                nrm /= (len / 3f);
//                nrm.Normalize();

//                return nrm;
//            }
//        }

//        sealed class ConnectFaceRebuildData
//        {
//            public FaceRebuildData faceRebuildData;
//            public List<int> newVertexIndexes;

//            public ConnectFaceRebuildData(FaceRebuildData faceRebuildData, List<int> newVertexIndexes)
//            {
//                this.faceRebuildData = faceRebuildData;
//                this.newVertexIndexes = newVertexIndexes;
//            }
//        }
//    }
//}

