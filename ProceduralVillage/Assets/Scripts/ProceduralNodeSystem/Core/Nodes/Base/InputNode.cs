using GraphProcessor;

/// <summary>
/// EL nodo de entrada de los grafos procedurales
/// </summary>
public class InputNode : ProceduralNode
{
    /// <summary>
    /// El flow inicial
    /// </summary>
    [Output("Start flow")]
    public GraphFlow StartFlow;

    protected override void Process()
    {
        if (StartFlow == null) StartFlow = new GraphFlow();
    }
}
 