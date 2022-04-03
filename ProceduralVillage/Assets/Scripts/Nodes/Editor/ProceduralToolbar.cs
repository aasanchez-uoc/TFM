using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProceduralToolbar : ToolbarView
{
    new ProceduralGraphView graphView => base.graphView as ProceduralGraphView;

    public ProceduralToolbar(BaseGraphView graphView) : base(graphView)
    {

    }

	protected override void AddButtons()
	{
		// Left buttons
		AddButton("Process", Process, left: true);

		AddSeparator(5);

		AddButton("Fit view", () => graphView.FrameAll());

	}

	void AddProcessButton()
	{
	}

	void Process()
	{
		EditorApplication.delayCall += () => graphView.ProcessGraph();
	}

}
