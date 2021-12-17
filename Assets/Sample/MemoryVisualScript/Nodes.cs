using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Memory;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;

public interface IVisualNode : IMemoryNode
{
    float GetValue(Graph graph, string port);
}

public abstract class VisualNode : MemoryNode, IVisualNode
{
    public abstract float GetValue(Graph graph, string port);

    public override bool IsPortCompatible(Graph graph, in PortId start, in PortId end)
    {
        var startNode = graph.FindNodeByPort(start);
        var endNode = graph.FindNodeByPort(end);
        return startNode.Id != endNode.Id && startNode.Inner is VisualNode && endNode.Inner is VisualNode;
    }

    protected IEnumerable<float> GetConnectedValues(Graph graph, string port)
    {
        return graph.FindConnectedPorts(new PortId(Id, port))
            .Select(connectedPort => ((IVisualNode)graph[connectedPort.NodeId].Inner).GetValue(graph, connectedPort.Name))
        ;
    }
}

[Serializable]
public class ValueNode : VisualNode
{
    [NodeProperty(OutputPort = nameof(_outPort))] public float Value;
    [NodePort] private static float _outPort;

    public override float GetValue(Graph graph, string port)
    {
        Assert.AreEqual(port, nameof(_outPort));
        return Value;
    }
}

[Serializable]
public class MultipleValueNode : VisualNode
{
    [NodeProperty(OutputPort = nameof(_outPort1))] public float Value1;
    [NodePort] private static float _outPort1;

    [NodeProperty(OutputPort = nameof(_outPort2))] public float Value2;
    [NodePort] private static float _outPort2;

    public override float GetValue(Graph graph, string port)
    {
        return port switch
        {
            nameof(_outPort1) => Value1,
            nameof(_outPort2) => Value2,
            _ => throw new NotImplementedException()
        };
    }
}

[Serializable, NodeTitle]
public class AddNode : VisualNode
{
    [NodeProperty(InputPort = nameof(_inputPort), OutputPort = nameof(_outputPort), HideLabel = true, HideValue = true)]
    private const int _ = 0;

    [NodePort(Capacity = Port.Capacity.Multi)]
    private static float _outputPort;

    [NodePort(Capacity = Port.Capacity.Multi)]
    public static float _inputPort;

    public override float GetValue(Graph graph, string port)
    {
        Assert.AreEqual(port, nameof(_outputPort));
        return GetConnectedValues(graph, nameof(_inputPort)).Sum();
    }
}

[Serializable, NodeTitle]
public class PrintNode : VisualNode
{
    [NodePort(Direction = Direction.Input)]
    private static float _input;

    public override float GetValue(Graph graph, string port)
    {
        var result = GetConnectedValues(graph, nameof(_input)).SingleOrDefault();
        Debug.Log($"value = {result}");
        return result;
    }
}