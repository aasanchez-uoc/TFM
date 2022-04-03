using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


[NodeCustomEditor(typeof(OutputNode))]
public class OutputNodeView : BaseNodeView
{

	public override void Enable(bool fromInspector)
	{
		capabilities &= ~Capabilities.Deletable;
		capabilities &= ~Capabilities.Copiable;
		base.Enable(fromInspector);

	}


}
