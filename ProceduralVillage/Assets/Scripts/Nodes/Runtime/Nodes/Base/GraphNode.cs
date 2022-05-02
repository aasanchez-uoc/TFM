using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphNode : BaseFlowNode
{
    [Input]
    public List<object> graphInputs = new List<object>();

    private ProceduralGraph subGraph;
    private ProceduralGraphProcessor processor;


	public void Init(ProceduralGraph graph)
    {
        subGraph = graph;
        processor = ProcessorManager.GetProcessor(subGraph);
    }

	[CustomPortInput(nameof(InputFlows), typeof(GraphFlow), allowCast = true)]
	public void GetInputs(List<SerializableEdge> edges)
	{
		InputFlows = edges.Select(e => (GraphFlow)e.passThroughBuffer);
	}

	[CustomPortBehavior(nameof(graphInputs))]
	public IEnumerable<PortData> ListGraphParameters(List<SerializableEdge> edges)
	{

		foreach (var p in subGraph?.exposedParameters)
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
		if (subGraph != null)
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
			subGraph.UpdateExposedParameter(propID, edge.passThroughBuffer);

		}
	}


	protected override void Process(int inputIndex)
    {
		if (InputFlows == null) return;
		GraphFlow InputFlow = InputFlows.ToList()[inputIndex];
		if (InputFlow?.CurrentGameObject != null)
		{
			subGraph.inputNode.StartFlow = InputFlow;
			if (processor == null) processor = ProcessorManager.GetProcessor(subGraph);
			processor?.Run();

			OutputFlow = subGraph.inputNode.StartFlow;
		}
    }
}
