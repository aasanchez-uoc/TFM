using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjectFromGraph : MonoBehaviour
{

    public ProceduralGraph GraphAsset;


    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate()
    {

        int childs = transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        ProceduralGraph graph = Instantiate(GraphAsset);
        GraphFlow flow = new GraphFlow();
        flow.CurrentGameObject = gameObject;
        graph.inputNode.StartFlow = flow;
        ProceduralGraphProcessor processor = ProcessorManager.GetProcessor(graph);
        processor?.Run();

    }
}
