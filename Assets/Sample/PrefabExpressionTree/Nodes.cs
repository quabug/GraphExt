using System;
using System.Linq;
using GraphExt;
using GraphExt.Prefab;
using UnityEngine;

public abstract class ExpressionTreeNode : INode
{
    public NodeId Id { get; set; }

    public bool IsPortCompatible(PrefabGraphBackend graph, in PortId start, in PortId end)
    {
        return true;
    }

    public void OnConnected(PrefabGraphBackend graph, in PortId start, in PortId end)
    {
    }

    public void OnDisconnected(PrefabGraphBackend graph, in PortId start, in PortId end)
    {
    }

    public abstract float GetValue(PrefabGraphBackend graph);

    protected float GetConnectedValue(PrefabGraphBackend graph, string port)
    {
        return graph.FindConnectedPorts(this, port)
            .Select(connectedPort => ((ExpressionTreeNode)graph.PrefabNodeMap[connectedPort.NodeId]).GetValue(graph))
            .Single()
        ;
    }
}

[Serializable]
public class ConstNode : ExpressionTreeNode
{
    [NodeProperty(OutputPort = nameof(_out)), SerializeField] private float _value;
    [NodePort] private static float _out;
    public override float GetValue(PrefabGraphBackend graph) => _value;
}

[Serializable]
public class AddNode : ExpressionTreeNode
{
    [NodePort(Direction = PortDirection.Input, Capacity = PortCapacity.Single, HideLabel = true)] private static float _in;
    [NodePort(Direction = PortDirection.Output, Capacity = PortCapacity.Single, HideLabel = true)] private static float _out1;
    [NodePort(Direction = PortDirection.Output, Capacity = PortCapacity.Single, HideLabel = true)] private static float _out2;
    public override float GetValue(PrefabGraphBackend graph) => GetConnectedValue(graph, nameof(_out1)) + GetConnectedValue(graph, nameof(_out2));
}

[Serializable]
public class AbsNode : ExpressionTreeNode
{
    [NodePort(Direction = PortDirection.Input, Capacity = PortCapacity.Single, HideLabel = true)] private static float _in;
    [NodePort(Direction = PortDirection.Output, Capacity = PortCapacity.Single, HideLabel = true)] private static float _out;
    public override float GetValue(PrefabGraphBackend graph) => GetConnectedValue(graph, nameof(_out));
}