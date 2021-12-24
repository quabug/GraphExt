using System;
using System.Linq;
using GraphExt;
using GraphExt.Prefab;
using UnityEngine;

public abstract class ExpressionTreeNode : INode
{
    public NodeId Id { get; set; }

    public bool IsPortCompatible(PrefabGraphBackend graph, in EdgeId connection)
    {
        return true;
    }

    public void OnConnected(PrefabGraphBackend graph, in EdgeId connection)
    {
    }

    public void OnDisconnected(PrefabGraphBackend graph, in EdgeId connection)
    {
    }

    public abstract float GetValue(PrefabGraphBackend graph);

    protected float GetConnectedValue(PrefabGraphBackend graph, string port)
    {
        return graph.FindConnectedPorts(new PortId(Id, port))
            .Select(connectedPort => ((ExpressionTreeNode)graph.GetNodeComponent(connectedPort.NodeId).Node).GetValue(graph))
            .Single()
        ;
    }
}

[Serializable]
public class ConstNode : ExpressionTreeNode
{
    [NodeProperty(InputPort = nameof(_in), Name = "const"), SerializeField] private float _value;
    [NodePort] private static float _in;
    public override float GetValue(PrefabGraphBackend graph) => _value;
}

[Serializable]
public class AddNode : ExpressionTreeNode
{
    [NodeProperty(OutputPort = nameof(_out1), InputPort = nameof(_in), HideValue = true, Name = "add")] private static int _;
    [NodePort(Direction = PortDirection.Input, Capacity = PortCapacity.Single, HideLabel = true)] private static float _in;
    [NodePort(Direction = PortDirection.Output, Capacity = PortCapacity.Single, HideLabel = true)] private static float _out1;
    [NodePort(Direction = PortDirection.Output, Capacity = PortCapacity.Single, HideLabel = true)] private static float _out2;
    public override float GetValue(PrefabGraphBackend graph) => GetConnectedValue(graph, nameof(_out1)) + GetConnectedValue(graph, nameof(_out2));
}

[Serializable]
public class AbsNode : ExpressionTreeNode
{
    [NodeProperty(OutputPort = nameof(_out), InputPort = nameof(_in), HideValue = true, Name = "abs")] private static int _;
    [NodePort(Capacity = PortCapacity.Single)] private static float _in;
    [NodePort(Capacity = PortCapacity.Single)] private static float _out;
    public override float GetValue(PrefabGraphBackend graph) => GetConnectedValue(graph, nameof(_out));
}