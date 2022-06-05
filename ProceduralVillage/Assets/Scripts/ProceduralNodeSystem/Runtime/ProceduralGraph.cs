using GraphProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// El grafo procedural que contendrá todos los nodos y paámetros
/// </summary>
[System.Serializable, CreateAssetMenu(fileName = "Procedural Graph", menuName = "Procedural/ProceduralGraph", order = 1)]
public class ProceduralGraph : BaseGraph
{
    #region Atributos privados
    [System.NonSerialized]
    InputNode _inputNode;
    #endregion

    #region Atributos públicos
    /// <summary>
    /// Booelano para registrar si estamos mostrando el panel de parametros o no
    /// </summary>
    public bool showParameterView;
    #endregion

    #region Constructor
    public ProceduralGraph()
    {
        base.onEnabled += Enabled;
    }
    #endregion

    #region Propiedades públicas
    /// <summary>
    /// El nodo inicial del grafo procedural
    /// </summary>
    public InputNode inputNode
    {
        get
        {
            if (_inputNode == null)
                _inputNode = nodes.FirstOrDefault(n => n is InputNode) as InputNode;

            return _inputNode;
        }
        set => _inputNode = value;
    }
    #endregion

    void Enabled()
    {
        //Creamos el nodo inicial
        if (inputNode == null)
            inputNode = AddNode(BaseNode.CreateFromType<InputNode>(new Vector2(0, 100))) as InputNode;
    }
}
