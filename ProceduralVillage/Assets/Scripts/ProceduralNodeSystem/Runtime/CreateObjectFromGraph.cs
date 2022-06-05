using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase encargada de generar el resultado en tiempo de ejecuci�n
/// </summary>
public class CreateObjectFromGraph : MonoBehaviour
{
    #region Atributos p�blicos
    /// <summary>
    /// El asset de ProceduralGraph se desea generar
    /// </summary>
    public ProceduralGraph GraphAsset;

    /// <summary>
    /// Booleano que indica si generaremos el resultado auto�ticamente al iniciar el script
    /// </summary>
    public bool GenerateOnStart = false;
    #endregion

    #region Atributos privados
    ProceduralGraph graph;
    #endregion

    void Awake()
    {
        graph = Instantiate(GraphAsset);
        if(GenerateOnStart) Generate();
    }

    /// <summary>
    /// M�todo que genera 
    /// </summary>
    public void Generate()
    {
        //Eliminamos todo el contenido previo de los hijos
        int childs = transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        //Creamos el graphflow inicial
        GraphFlow flow = new GraphFlow();
        flow.CurrentGameObject = gameObject;
        graph.inputNode.StartFlow = flow;
        ProceduralGraphProcessor processor = ProcessorManager.GetProcessor(graph);
        try
        {
            //Ejecutamos el grafo
            processor?.Run();
        }
        catch
        {
            //Si algo sale mal, intentamos volver a generarlo
            Generate();
        }
    }

    /// <summary>
    /// M�todo que nos permite establecer los par�metros de grafo
    /// </summary>
    /// <param name="name">El nombre del par�metro</param>
    /// <param name="value">EL valor del par�metro</param>
    public void SetParameterValue(string name, object value)
    {
        graph.SetParameterValue(name, value);
    }
}
