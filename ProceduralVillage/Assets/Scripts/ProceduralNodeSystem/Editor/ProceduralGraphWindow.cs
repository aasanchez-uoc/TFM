using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		//var graphWindow = EditorWindow.CreateWindow<ProceduralGraphWindow>();
		var types = new List<Type>()
		{ 
			typeof(Editor).Assembly.GetType("UnityEditor.ConsoleWindow"),
			typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser"),
			typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow"),
			typeof(SceneView),
			typeof(Editor).Assembly.GetType("UnityEditor.GameView"),
			typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow"),
			typeof(Editor).Assembly.GetType("UnityEditor.ConsoleWindow")
		};

		types.AddRange(AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(EditorWindow))));
		var graphWindow = GetWindow<ProceduralGraphWindow>(types.ToArray());

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

	protected override void InitializeGraphView(BaseGraphView view)
	{
		this.view.Initialize();
	}


	protected override void Update()
    {

    }

	protected override void OnDestroy()
	{
		view?.Disable();
		view?.Dispose();
	}

	public ProceduralGraph GetCurrentGraph() => graph as ProceduralGraph;

}
