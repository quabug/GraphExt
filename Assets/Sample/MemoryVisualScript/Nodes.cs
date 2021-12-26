using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEngine.Assertions;

public interface IVisualNode : IMemoryNode
{
    float GetValue([NotNull] GraphRuntime<IMemoryNode> graph);
    float GetValue([NotNull] GraphRuntime<IMemoryNode> graph, [NotNull] string port);
}

public abstract class VisualNode : IVisualNode
{
    public abstract float GetValue(GraphRuntime<IMemoryNode> graph);
    public abstract float GetValue(GraphRuntime<IMemoryNode> graph, string port);

    public virtual bool IsPortCompatible(GraphRuntime<IMemoryNode> graph, in PortId start, in PortId end)
    {
        var startNode = graph.GetNodeByPort(start);
        var endNode = graph.GetNodeByPort(end);
        return start.NodeId != end.NodeId && startNode is VisualNode && endNode is VisualNode;
    }

    protected IEnumerable<float> GetConnectedValues(GraphRuntime<IMemoryNode> graph, string port)
    {
        return graph.FindConnectedPorts(this, port)
            .Select(connectedPort => ((IVisualNode)graph.GetNodeByPort(connectedPort)).GetValue(graph, connectedPort.Name))
        ;
    }

    public virtual void OnConnected(GraphRuntime<IMemoryNode> graph, in PortId start, in PortId end) {}
    public virtual void OnDisconnected(GraphRuntime<IMemoryNode> graph, in PortId start, in PortId end) {}
}

public class ValueNode : VisualNode
{
    [NodeProperty(OutputPort = nameof(_outPort))] public float Value;
    [NodePort] private static float _outPort;

    public override float GetValue(GraphRuntime<IMemoryNode> graph, string port)
    {
        Assert.IsTrue(port == nameof(_outPort));
        return GetValue(graph);
    }

    public override float GetValue(GraphRuntime<IMemoryNode> graph)
    {
        return Value;
    }
}

[NodeTitle(ConstTitle = "multi")]
public class MultipleValueNode : VisualNode
{
    [NodeProperty(OutputPort = nameof(_outPort1))] public float Value1;
    [NodePort] private static float _outPort1;

    [NodeProperty(OutputPort = nameof(_outPort2))] public float Value2;
    [NodePort] private static float _outPort2;

    public override float GetValue(GraphRuntime<IMemoryNode> graph)
    {
        return Value1;
    }

    public override float GetValue(GraphRuntime<IMemoryNode> graph, string port)
    {
        return port switch
        {
            nameof(_outPort1) => Value1,
            nameof(_outPort2) => Value2,
            _ => throw new NotImplementedException()
        };
    }
}

public class AddNode : VisualNode
{
    [NodeProperty(InputPort = nameof(_inputPort), OutputPort = nameof(_outputPort), HideValue = true)]
    private const int Add = 0;
    [NodePort] private static float _outputPort;
    [NodePort(Capacity = PortCapacity.Multi)] public static float _inputPort;

    public override float GetValue(GraphRuntime<IMemoryNode> graph)
    {
        return GetConnectedValues(graph, nameof(_inputPort)).Sum();
    }

    public override float GetValue(GraphRuntime<IMemoryNode> graph, string port)
    {
        Assert.IsTrue(port == nameof(_outputPort));
        return GetValue(graph);
    }
}