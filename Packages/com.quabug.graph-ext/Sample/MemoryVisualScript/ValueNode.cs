using System;
using System.Linq;
using GraphExt.Memory;

public interface IVisualNode : IMemoryNode
{
    float Calculate(Graph graph);
}

public struct VisualPort : IMemoryPort
{
    public Action<Graph, IMemoryPort> OnConnected { get; }
    public Action<Graph, IMemoryPort> OnDisconnected { get; }
    public bool IsCompatible(Graph graph, IMemoryPort port) => port is VisualPort;
}

[Serializable]
public class ValueNode : IVisualNode
{
    [NodeProperty] public float Value;

    [NodePort(typeof(float), Direction = NodePortDirection.Output, AllowMultipleConnections = true)]
    public VisualPort Output;

    public float Calculate(Graph graph)
    {
        return Value;
    }
}

[Serializable, NodeTitle]
public class AddNode : IVisualNode
{
    [NodePort(typeof(float), Direction = NodePortDirection.Output, AllowMultipleConnections = true)]
    public VisualPort Output;

    [NodePort(typeof(float), Direction = NodePortDirection.Input)]
    public VisualPort First;

    [NodePort(typeof(float), Direction = NodePortDirection.Input)]
    public VisualPort Second;

    public float Calculate(Graph graph)
    {
        return ((IVisualNode)graph.FindConnectedNode(First).Single()).Calculate(graph) +
            ((IVisualNode)graph.FindConnectedNode(Second).Single()).Calculate(graph);
    }
}

[Serializable, NodeTitle]
public class PrintNode : IVisualNode
{
    [NodePort(typeof(float), Direction = NodePortDirection.Input)]
    public VisualPort Input;

    public float Calculate(Graph graph)
    {
        var result = ((IVisualNode)graph.FindConnectedNode(Input).Single()).Calculate(graph);
        UnityEngine.Debug.Log(result);
        return result;
    }
}