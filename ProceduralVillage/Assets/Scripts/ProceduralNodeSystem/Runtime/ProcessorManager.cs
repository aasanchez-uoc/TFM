using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ProcessorManager 
{
	internal static Dictionary<ProceduralGraph, HashSet<ProceduralGraphProcessor>> processorInstances = new Dictionary<ProceduralGraph, HashSet<ProceduralGraphProcessor>>();

	public static ProceduralGraphProcessor GetProcessor(ProceduralGraph graph)
	{
		processorInstances.TryGetValue(graph, out var processorSet);
		if (processorSet == null)
			return new ProceduralGraphProcessor(graph);
		else
			return processorSet.FirstOrDefault(p => p != null);
	}

	public static void AddProcessor(ProceduralGraphProcessor processor, ProceduralGraph graph)
	{
        processorInstances.TryGetValue(graph , out var hashset);
        if (hashset == null)
            hashset = processorInstances[graph] = new HashSet<ProceduralGraphProcessor>();

		hashset.Add(processor);

    }
	public static void Remove(ProceduralGraph graph)
    {
		processorInstances.Remove(graph);
	}
}
