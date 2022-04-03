using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ProceduralGraphView : BaseGraphView
{
	public ProceduralGraphProcessor processor { get; private set; }
	public new ProceduralGraph graph => base.graph as ProceduralGraph;

	public bool Loading = true;
	GraphFlow generatedObject = null;

	public ProceduralGraphView(EditorWindow window) : base(window)
	{
		initialized += Initialize;
		Undo.undoRedoPerformed += ReloadGraph;
		RegisterCallback<DetachFromPanelEvent>(e => Disable());

		SetupZoom(0.05f, 32f);
	}

	public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
	{
		var compatiblePorts = new List<Port>();
		PortView startPortView = startPort as PortView;

		compatiblePorts.AddRange(ports.ToList().Where(p => {
			var portView = p as PortView;

			if (p.direction == startPort.direction)
				return false;

			if (p.node == startPort.node)
				return false;

			//Check if there is custom adapters for this assignation
			if (CustomPortIO.IsAssignable(startPort.portType, p.portType))
				return true;

			// Allow connection between RenderTexture and all texture types:
			Type startType = startPortView.portData.displayType ?? startPortView.portType;
			Type endType = portView.portData.displayType ?? portView.portType;
			if (startType == typeof(RenderTexture))
			{
				if (endType.IsSubclassOf(typeof(Texture)))
					return true;
			}
			if (endType == typeof(RenderTexture))
			{
				if (startType.IsSubclassOf(typeof(Texture)))
					return true;
			}

			//Check for type assignability
			if (!BaseGraph.TypesAreConnectable(startPort.portType, p.portType))
				return false;

			//Check if the edge already exists
			if (portView.GetEdges().Any(e => e.input == startPort || e.output == startPort))
				return false;

			return true;
		}));

		return compatiblePorts;
	}

		void Initialize()
	{

		processor = ProcessorManager.GetProcessor(graph);
		computeOrderUpdated += () => {
			processor.UpdateComputeOrder();
		};
		graph.onGraphChanges -= ProcessGraphWhenChanged;
		graph.onGraphChanges += ProcessGraphWhenChanged;
	}

	public void Disable()
	{
		Undo.undoRedoPerformed -= ReloadGraph;
		graph.onGraphChanges -= ProcessGraphWhenChanged;

		if (generatedObject != null) UnityEngine.Object.DestroyImmediate(generatedObject.CurrentGameObject);
	}


	void ProcessGraphWhenChanged(GraphChanges changes)
	{
		if (changes.addedEdge != null || changes.removedEdge != null
			|| changes.addedNode != null || changes.removedNode != null || changes.nodeChanged != null)
		{
				ProcessGraph();
		}
	}

	public void ProcessGraph()
	{
		try
		{
			if (generatedObject != null) UnityEngine.Object.DestroyImmediate(generatedObject.CurrentGameObject);
			generatedObject = new GraphFlow("Procedural Graph - Preview");
			graph.inputNode.StartFlow = generatedObject;
			processor?.Run();

		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}

		MarkDirtyRepaint();
	}

	void ReloadGraph()
	{
		graph.outputNode = null;
		ProcessGraph();
	}


}
