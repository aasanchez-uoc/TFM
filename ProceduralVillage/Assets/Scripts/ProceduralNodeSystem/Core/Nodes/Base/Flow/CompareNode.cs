using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

/// <summary>
/// Compara dos valores float y en función del resultado ejecuta un flow u otro
/// </summary>
[System.Serializable, NodeMenuItem("Flow/Compare Values")]
public class CompareValuesNode : BaseFlowNode
{
    [Output("False", allowMultiple = false)]
    public GraphFlow OtherOutputFlow;

    public CompareTypes CompareType = CompareTypes.Less;

    [Input("A"), ShowAsDrawer]
    public float a = 0;

    [Input("B"), ShowAsDrawer]
    public float b = 0;

    public override void Process(GraphFlow inputFlow)
    {
        OtherOutputFlow = null;
        OutputFlow = null;
        if (inputFlow != null)
        {
            if (Compare(a, b, CompareType)) OutputFlow = inputFlow;
            else OtherOutputFlow = inputFlow;
        }
        else OtherOutputFlow = inputFlow;
    }

    private bool Compare(float a, float b, CompareTypes comparison)
    {
        switch (comparison)
        {
            case CompareTypes.Less:
                return a < b;
            case CompareTypes.LessOrEqual:
                return a <= b;
            case CompareTypes.Equal:
                return a == b;
            case CompareTypes.GreaterOrEqual:
                return a >= b;
            case CompareTypes.Greater:
                return a > b;
            default:
                return false;
        }
    }

    public enum CompareTypes
    {
        Less,
        LessOrEqual,
        Equal,
        GreaterOrEqual,
        Greater
    }
}
