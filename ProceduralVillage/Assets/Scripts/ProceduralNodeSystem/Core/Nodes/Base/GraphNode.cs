using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Nodo que encapsula el contenido de otro grafo
/// </summary>
[System.Serializable]
public class GraphNode : BaseFlowNode
{
    [Input]
    public List<object> graphInputs = new List<object>();

	[SerializeReference, HideInInspector]
	public ProceduralGraph subGraphAsset;

	private ProceduralGraph subGraph;
    private ProceduralGraphProcessor processor;


	public void Init(ProceduralGraph graph)
    {
		subGraphAsset = graph;
        processor = ProcessorManager.GetProcessor(GetSubGraph());
		SetCustomName(graph.name);
    }

	[CustomPortBehavior(nameof(graphInputs))]
	public IEnumerable<PortData> ListGraphParameters(List<SerializableEdge> edges)
	{

		foreach (var p in GetSubGraph()?.exposedParameters)
		{
			yield return new PortData
			{
				displayName = p.name,
				identifier = p.guid,			
				displayType = p.GetValueType(),
			};
		}
	}

	[CustomPortInput(nameof(graphInputs), typeof(object))]
	protected void GetGraphInputs(List<SerializableEdge> edges)
	{
		if (GetSubGraph() != null)
			AssignPropertiesFromEdges(edges);
	}

	protected void AssignPropertiesFromEdges(List<SerializableEdge> edges)
	{
		// Update material settings when processing the graph:
		foreach (var edge in edges)
		{
			// Just in case something bad happened in a node
			if (edge.passThroughBuffer == null)
				continue;

			string propID= edge.inputPort.portData.identifier;
			GetSubGraph().UpdateExposedParameter(propID, edge.passThroughBuffer);

		}
	}
	public override void Process(GraphFlow InputFlow)
	{
		if (InputFlow?.CurrentGameObject != null)
		{
			ProceduralGraph currSubGraph = GetSubGraph();
			currSubGraph.inputNode.StartFlow = InputFlow;
			if (processor == null) processor = ProcessorManager.GetProcessor(currSubGraph);
			processor?.Run();

			OutputFlow = currSubGraph.inputNode.StartFlow;
		}
	}

	private ProceduralGraph GetSubGraph()
    {
		if(subGraph == null && subGraphAsset != null) subGraph = Object.Instantiate(subGraphAsset);
		return subGraph;
	}
}
