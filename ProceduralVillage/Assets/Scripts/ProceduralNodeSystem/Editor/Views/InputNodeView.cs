using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


[NodeCustomEditor(typeof(InputNode))]
public class InputNodeView : BaseNodeView
{

	public override void Enable(bool fromInspector)
	{
		capabilities &= ~Capabilities.Deletable;
		capabilities &= ~Capabilities.Copiable;
		base.Enable(fromInspector);

	}


}
