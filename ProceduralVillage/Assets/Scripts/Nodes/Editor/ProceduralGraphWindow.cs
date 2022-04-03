using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class ProceduralGraphWindow : BaseGraphWindow
{
	internal ProceduralGraphView view;

	public static ProceduralGraphWindow Open(ProceduralGraph graph)
	{
		// Focus the window if the graph is already opened
		var proceduralWindows = Resources.FindObjectsOfTypeAll<ProceduralGraphWindow>();
		foreach (var window in proceduralWindows)
		{
			if (window.graph == graph)
			{
				window.Show();
				window.Focus();
				return window;
			}
		}

		var graphWindow = EditorWindow.CreateWindow<ProceduralGraphWindow>();

		graphWindow.Show();
		graphWindow.Focus();

		graphWindow.InitializeGraph(graph);
		return graphWindow;
	}

	protected override void InitializeWindow(BaseGraph graph)
	{
		if (view != null)
		{
			view.Disable();
			view.Dispose();
		}

		var title = "Procedural Graph";
		titleContent = new GUIContent(title);

		view = new ProceduralGraphView(this);

		rootView.Add(view);

		view.Add(new ProceduralToolbar(view));
	}

	protected override void OnDestroy()
	{
		view?.Disable();
		view?.Dispose();
	}

	public ProceduralGraph GetCurrentGraph() => graph as ProceduralGraph;

}
