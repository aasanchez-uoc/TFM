using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase encargada de generar el resultado en tiempo de ejecución
/// </summary>
public class CreateObjectFromGraph : MonoBehaviour
{
    #region Atributos públicos
    /// <summary>
    /// El asset de ProceduralGraph se desea generar
    /// </summary>
    public ProceduralGraph GraphAsset;

    /// <summary>
    /// Booleano que indica si generaremos el resultado autoáticamente al iniciar el script
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
    /// Método que genera 
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
    /// Método que nos permite establecer los parámetros de grafo
    /// </summary>
    /// <param name="name">El nombre del parámetro</param>
    /// <param name="value">EL valor del parámetro</param>
    public void SetParameterValue(string name, object value)
    {
        graph.SetParameterValue(name, value);
    }
}
