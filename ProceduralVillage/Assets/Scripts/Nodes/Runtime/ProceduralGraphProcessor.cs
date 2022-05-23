using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComputeOrderInfo
{
    public Dictionary<BaseNode, MultiFlowInfo> MultiFlowNodes = new Dictionary<BaseNode, MultiFlowInfo>();

    public void Clear() => MultiFlowNodes.Clear();
}

public class MultiFlowInfo
{
    public int index = 0;
    public List<BaseNode> DependentNodes;
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

    int maxLoopLevel = 10;

    internal new ProceduralGraph graph => base.graph as ProceduralGraph;

    List<BaseNode> nodesToProcess = new List<BaseNode>();

    public ComputeOrderInfo info { get; private set; } = new ComputeOrderInfo();

    public ProceduralGraphProcessor(ProceduralGraph graph) : base(graph)
    {
        ProcessorManager.AddProcessor(this, graph);
    }

    public override void Run()
    {
        //RunNodeList(graph.graphOutputs, true);
        //RunNodeList(new List<BaseNode> { graph.inputNode }, false);

        UpdateComputeOrder();
        ProcessGraphNodes(nodesToProcess, true);
    }

    public void RunOnChagedNode(BaseNode node)
    {
        //RunNodeList(new List<BaseNode> { node }, false);
        //RunNodeList(new List<BaseNode> { graph.inputNode }, true);
        UpdateComputeOrder();
        List<BaseNode> nodes = nodesToProcess.Where(x => x is BaseFlowNode).ToList();
        nodes.Add(graph.inputNode);
        List<BaseNode> deps = GetNodeDependencies(node, false);

        foreach(BaseNode n in deps)
        {
            if (!nodes.Contains(n)) nodes.Add(n);
        }

        ProcessGraphNodes(nodes.OrderBy(x => x.computeOrder), false);

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
        nodesToProcess.Clear();

        var sortedNodes = graph.nodes.OrderBy(n => n.computeOrder).ToList();

        foreach(BaseNode node in sortedNodes)
        {
            if(node is BaseFlowNode flowNode)
            {
                bool multiFlow = false;
                //We search for the flow input port
                foreach (var port in node.inputPorts)
                {
                    //check if there are multiple input flows
                    if (port.fieldName == "InputFlows")
                    {
                        if(port.GetEdges().Count > 1)
                        {
                            multiFlow = true;
                            //List<BaseNode> depNodes = GetNodeDependencies(node, false);
                            List<BaseNode> depNodes = new List<BaseNode>();
                            depNodes.Add(node);

                            var edges = port.GetEdges();
                            for (int i = 0; i < edges.Count; i++)
                            {
                                MultiFlowInfo nodeInfo = new MultiFlowInfo();
                                nodeInfo.index = i;
                                nodeInfo.DependentNodes = depNodes;
                                info.MultiFlowNodes.Add(edges[i].outputNode, nodeInfo);
                            }
                        }

                    }
                }
                if(!multiFlow && node.computeOrder >= 0) nodesToProcess.Add(node);

            }
            else
            {
                if(node.computeOrder >= 0) nodesToProcess.Add(node);
            }
        }

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
                    if (graph.nodes.Contains(dep) && nodesToProcess.Contains(dep))
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

    void ProcessNode(BaseNode node, int index = 0, Dictionary<BaseNode, int> loopLevel = null )
    {
        if (!node.canProcess)
            return;
        if( node is BaseFlowNode flowNode)
        {
            if (loopLevel == null) loopLevel = new Dictionary<BaseNode, int>();
            if (!loopLevel.TryGetValue(node, out int currLevel)) currLevel = 0;
            if (currLevel != maxLoopLevel)
            {
                flowNode.inputPorts.PullDatas();
                if (flowNode.CountInputsOnEdge(index) > 1)
                {
                    //flowNode.
                    int n = flowNode.CountInputsOnEdge(index);
                    for(int i = 1; i < n; i++)
                    {
                        flowNode.OnProcess(index, i);
                        ProcessNodeFromInput(flowNode);
                    }
                }

                flowNode.OnProcess(index);
                if (info.MultiFlowNodes.ContainsKey(node))
                {
                    ProcessNodeFromInput(flowNode);
                    //info.MultiFlowNodes.TryGetValue(node, out MultiFlowInfo nodeInfo);
                    //foreach (BaseNode subNode in nodeInfo.DependentNodes)
                    //{
                    //    loopLevel[subNode] = currLevel + 1;
                    //    ProcessNode(subNode, nodeInfo.index, loopLevel);
                    //    loopLevel[subNode] = currLevel;
                    //}
                }
            }
        }
        else
        {
            node.OnProcess();
        }

    }

    public void ProcessNodeFromInput(BaseFlowNode node)
    {
        List<BaseNode> depNodes = node.GetOutputNodes().ToList();
        //foreach (BaseNode depNode in depNodes)
        //{
        //    if (depNode is BaseFlowNode depFlowNode)
        //    {
        //        if (node.OutputFlow is GraphFlow singleFlow)
        //        {
        //            depFlowNode.Process(singleFlow);
        //            ProcessNodeFromInput(depFlowNode);
        //        }
        //        else if (node.OutputFlow is IEnumerable<GraphFlow> multiFlow)
        //        {
        //            foreach (GraphFlow flow in multiFlow)
        //            {
        //                depFlowNode.Process(flow);
        //                ProcessNodeFromInput(depFlowNode);
        //            }
        //        }
        //    }
        //}
        node.outputPorts.PushDatas();
        foreach (var port in node.outputPorts)
        {
            if( typeof(GraphFlow).IsAssignableFrom(port.fieldInfo.FieldType))
            {
                foreach (var edge in port.GetEdges())
                {
                    if(edge.inputNode is BaseFlowNode depFlowNode)
                    {
                        GraphFlow flow = (GraphFlow) edge.passThroughBuffer;
                        depFlowNode.Process(flow);
                        ProcessNodeFromInput(depFlowNode);
                    }

                }
            }

            if (typeof(IEnumerable<GraphFlow>).IsAssignableFrom(port.fieldInfo.FieldType))
            {
                foreach (var edge in port.GetEdges())
                {
                    if (edge.inputNode is BaseFlowNode depFlowNode)
                    {
                        List<GraphFlow> multiFlow = (List<GraphFlow>)edge.passThroughBuffer;
                        foreach (GraphFlow flow in multiFlow)
                        {
                            depFlowNode.Process(flow);
                            ProcessNodeFromInput(depFlowNode);
                        }
                    }

                }

            }


        }

    }
}
