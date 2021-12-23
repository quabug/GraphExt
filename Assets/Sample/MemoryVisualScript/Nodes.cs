using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Memory;
using JetBrains.Annotations;
using UnityEngine.Assertions;

public interface IVisualNode : IMemoryNode
{
    float GetValue([NotNull] MemoryGraphBackend graph);
    float GetValue([NotNull] MemoryGraphBackend graph, [NotNull] string port);
}

public abstract class VisualNode : MemoryNode, IVisualNode
{
    public abstract float GetValue(MemoryGraphBackend graph);
    public abstract float GetValue(MemoryGraphBackend graph, string port);

    public override bool IsPortCompatible(MemoryGraphBackend graph, in PortId start, in PortId end)
    {
        var startNode = graph.GetMemoryNodeByPort(start);
        var endNode = graph.GetMemoryNodeByPort(end);
        return start.NodeId != end.NodeId && startNode is VisualNode && endNode is VisualNode;
    }

    protected IEnumerable<float> GetConnectedValues(MemoryGraphBackend graph, string port)
    {
        return graph.FindConnectedPorts(this, port)
            .Select(connectedPort => ((IVisualNode)graph.GetMemoryNodeByPort(connectedPort)).GetValue(graph, connectedPort.Name))
        ;
    }
}

public class ValueNode : VisualNode
{
    [NodeProperty(OutputPort = nameof(_outPort))] public float Value;
    [NodePort] private static float _outPort;

    public override float GetValue(MemoryGraphBackend graph, string port)
    {
        Assert.IsTrue(port == nameof(_outPort));
        return GetValue(graph);
    }

    public override float GetValue(MemoryGraphBackend graph)
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

    public override float GetValue(MemoryGraphBackend graph)
    {
        return Value1;
    }

    public override float GetValue(MemoryGraphBackend graph, string port)
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

    public override float GetValue(MemoryGraphBackend graph)
    {
        return GetConnectedValues(graph, nameof(_inputPort)).Sum();
    }

    public override float GetValue(MemoryGraphBackend graph, string port)
    {
        Assert.IsTrue(port == nameof(_outputPort));
        return GetValue(graph);
    }
}