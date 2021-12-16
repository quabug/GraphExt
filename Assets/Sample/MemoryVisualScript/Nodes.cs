using System;
using System.Linq;
using GraphExt.Memory;

public interface IVisualNode : IMemoryNode
{
    float Calculate(Graph graph);
}

public class VisualPort : MemoryPort
{
    public override bool IsCompatible(Graph graph, IMemoryPort port) => port is VisualPort;
}

[Serializable]
public class ValueNode : IVisualNode
{
    public Guid Id { get; } = Guid.NewGuid();

    [NodeProperty] public float Value;

    [NodePort(typeof(float), Direction = NodePortDirection.Output, AllowMultipleConnections = true)]
    public VisualPort Output = new VisualPort();

    public float Calculate(Graph graph)
    {
        return Value;
    }
}

[Serializable, NodeTitle]
public class AddNode : IVisualNode
{
    public Guid Id { get; } = Guid.NewGuid();

    [NodePort(typeof(float), Direction = NodePortDirection.Output, AllowMultipleConnections = true)]
    public VisualPort Output = new VisualPort();

    [NodePort(typeof(float), Direction = NodePortDirection.Input)]
    public VisualPort First = new VisualPort();

    [NodePort(typeof(float), Direction = NodePortDirection.Input)]
    public VisualPort Second = new VisualPort();

    public float Calculate(Graph graph)
    {
        return ((IVisualNode)graph.FindConnectedNode(First.Id).Single()).Calculate(graph) +
            ((IVisualNode)graph.FindConnectedNode(Second.Id).Single()).Calculate(graph);
    }
}

[Serializable, NodeTitle]
public class PrintNode : IVisualNode
{
    public Guid Id { get; } = Guid.NewGuid();

    [NodePort(typeof(float), Direction = NodePortDirection.Input)]
    public VisualPort Input = new VisualPort();

    public float Calculate(Graph graph)
    {
        var result = ((IVisualNode)graph.FindConnectedNode(Input.Id).Single()).Calculate(graph);
        UnityEngine.Debug.Log(result);
        return result;
    }
}