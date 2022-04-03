using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComputeOrderInfo
{
    public Dictionary<BaseNode, int> forLoopLevel = new Dictionary<BaseNode, int>();

    public void Clear() => forLoopLevel.Clear();
}

struct ProcessingScope : IDisposable
{
    ProceduralGraphProcessor processor;

    public ProcessingScope(ProceduralGraphProcessor processor)
    {
        this.processor = processor;
        processor.isProcessing++;
    }

    public void Dispose() => processor.isProcessing--;
}

public class ProceduralGraphProcessor : BaseGraphProcessor , IDisposable
{
    internal int isProcessing = 0;

    internal new ProceduralGraph graph => base.graph as ProceduralGraph;

    public ComputeOrderInfo info { get; private set; } = new ComputeOrderInfo();

    public ProceduralGraphProcessor(ProceduralGraph graph) : base(graph)
    {
        ProcessorManager.AddProcessor(this, graph);
    }

    public override void Run()
    {
        RunNodeList(graph.graphOutputs, true);
    }

    private void RunNodeList(IEnumerable<BaseNode> nodes, bool outputs)
    {
        using (new ProcessingScope(this))
        {
            UpdateComputeOrder();
            ProcessGraphNodes(nodes, outputs);
        }
    }

    public void Dispose() => ProcessorManager.Remove(graph);

    public override void UpdateComputeOrder()
    {
        info.Clear();

        var sortedNodes = graph.nodes.Where(n => n.computeOrder >= 0).OrderBy(n => n.computeOrder).ToList();
    }

    void ProcessGraphNodes(IEnumerable<BaseNode> graphOutputs, bool outputs)
    {
        using (new ProcessingScope(this))
        {
            HashSet<BaseNode> finalNodes = new HashSet<BaseNode>();

            foreach (var node in graphOutputs)
            {
                List<BaseNode> nodes = GetNodeDependencies(node, outputs);
                foreach (var dep in nodes)
                {
                    if (graph.nodes.Contains(dep))
                        finalNodes.Add(dep);
                }
            }

            ProcessNodeList(finalNodes);
        }
    }

    List<BaseNode> GetNodeDependencies(BaseNode node, bool input = true)
    {
        HashSet<BaseNode> dependencies = new HashSet<BaseNode>();
        Stack<BaseNode> nodes = new Stack<BaseNode>(input ? node.GetInputNodes() : node.GetOutputNodes());

        dependencies.Add(node);

        while (nodes.Count > 0)
        {
            var child = nodes.Pop();

            if (!dependencies.Add(child))
                continue;

            foreach (var parent in ( input ? child.GetInputNodes() : child.GetOutputNodes() ))
                nodes.Push(parent);

        }

        return dependencies.OrderBy(d => d.computeOrder).ToList();
    }



    void ProcessNodeList(HashSet<BaseNode> nodes)
    {
        var sortedNodes = nodes.Where(n => n.computeOrder >= 0).OrderBy(n => n.computeOrder).ToList();
        for (int executionIndex = 0; executionIndex < sortedNodes.Count; executionIndex++)
        {
            var node = sortedNodes[executionIndex];
            ProcessNode(node);
        }
    }

    void ProcessNode(BaseNode node)
    {
        if (node.computeOrder < 0 || !node.canProcess)
            return;
        node.OnProcess();
    }
}
