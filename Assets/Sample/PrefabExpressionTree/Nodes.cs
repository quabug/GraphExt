using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Prefab;
using UnityEngine;

public abstract class ExpressionTreeNode : ITreeNode
{
    public NodeId Id { get; set; }

    [NodePort(Direction = PortDirection.Input, Capacity = PortCapacity.Single, HideLabel = true)] protected static float _in;
    [NodePort(Direction = PortDirection.Output, Capacity = PortCapacity.Multi, HideLabel = true)] protected static float _out;

    public virtual string InputPortName => nameof(_in);
    public virtual string OutputPortName => nameof(_out);

    public bool IsPortCompatible(PrefabGraphBackend graph, in PortId input, in PortId output)
    {
        return true;
    }

    public void OnConnected(PrefabGraphBackend graph, in PortId input, in PortId output)
    {
    }

    public void OnDisconnected(PrefabGraphBackend graph, in PortId input, in PortId output)
    {
    }

    public abstract float GetValue(PrefabGraphBackend graph);

    protected IEnumerable<float> GetConnectedValues(PrefabGraphBackend graph)
    {
        return graph.FindConnectedPorts(new PortId(Id, OutputPortName))
            .Select(connectedPort => ((ExpressionTreeNode)graph.GetNodeComponent(connectedPort.NodeId).Node).GetValue(graph))
        ;
    }
}

[Serializable]
public class ConstNode : ExpressionTreeNode
{
    [NodeProperty(InputPort = nameof(_in), Name = "const"), SerializeField] private float _value;
    public override float GetValue(PrefabGraphBackend graph) => _value;
}

[Serializable]
public class AddNode : ExpressionTreeNode
{
    [NodeProperty(OutputPort = nameof(_out), InputPort = nameof(_in), HideValue = true, Name = "add")] private static int _;
    public override float GetValue(PrefabGraphBackend graph) => GetConnectedValues(graph).Sum();
}

[Serializable]
public class AbsNode : ExpressionTreeNode
{
    [NodeProperty(OutputPort = nameof(_singleOut), InputPort = nameof(_in), HideValue = true, Name = "abs")] private static int _;
    [NodePort(Direction = PortDirection.Output, Capacity = PortCapacity.Single, HideLabel = true)] protected static float _singleOut;
    public override string OutputPortName => nameof(_singleOut);
    public override float GetValue(PrefabGraphBackend graph) => GetConnectedValues(graph).Single();
}