using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExposedParameterPropertyView : VisualElement
{
	protected ProceduralGraphView mixtureGraphView;

	public ExposedParameter parameter { get; private set; }

	public Toggle hideInInspector { get; private set; }

	public ExposedParameterPropertyView(BaseGraphView graphView, ExposedParameter param)
	{
		mixtureGraphView = graphView as ProceduralGraphView;
		parameter = param;

		var valueField = graphView.exposedParameterFactory.GetParameterValueField(param, (newValue) => {
			graphView.RegisterCompleteObjectUndo("Updated Parameter Value");
			param.value = newValue;
			graphView.graph.NotifyExposedParameterValueChanged(param);
			mixtureGraphView.ProcessGraph();
		});

		var field = graphView.exposedParameterFactory.GetParameterSettingsField(param, (newValue) => {
			param.settings = newValue as ExposedParameter.Settings;
		});

		Add(valueField);

		Add(field);
	}
}
