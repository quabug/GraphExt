using System.Collections.Generic;
using System.Linq;
using GraphExt;
using JetBrains.Annotations;

public interface IVisualNode : ITreeNode<GraphRuntime<IVisualNode>>
{
    float GetValue([NotNull] GraphRuntime<IVisualNode> graph);
}

public abstract class VisualNode : IVisualNode
{
    [NodePort(Hide = true, Direction = PortDirection.Output, Capacity = 2)] protected static float[] _Out;
    [NodePort(Hide = true, Direction = PortDirection.Input)] protected static float _In;
    public string InputPortName => nameof(_In);
    public string OutputPortName => nameof(_Out);

    public abstract float GetValue(GraphRuntime<IVisualNode> graph);

    public virtual bool IsPortCompatible(GraphRuntime<IVisualNode> graph, in PortId input, in PortId output)
    {
        var startNode = graph.GetNodeByPort(output);
        var endNode = graph.GetNodeByPort(input);
        return output.NodeId != input.NodeId && startNode is VisualNode && endNode is VisualNode;
    }

    protected IEnumerable<float> GetConnectedValues(GraphRuntime<IVisualNode> graph)
    {
        return graph.FindConnectedPorts(this, OutputPortName).Select(connectedPort => graph.GetNodeByPort(connectedPort).GetValue(graph));
    }

    public virtual void OnConnected(GraphRuntime<IVisualNode> graph, in PortId input, in PortId output) {}
    public virtual void OnDisconnected(GraphRuntime<IVisualNode> graph, in PortId input, in PortId output) {}
}

public class ValueNode : VisualNode
{
    [NodeProperty(InputPort = nameof(_In))] public float Value;
    public override float GetValue(GraphRuntime<IVisualNode> graph) => Value;
}

public class AddNode : VisualNode
{
    [NodeProperty(InputPort = nameof(_In), OutputPort = nameof(_Out), HideValue = true, Name = "add")]
    private const int _ = 0;
    public override float GetValue(GraphRuntime<IVisualNode> graph) => GetConnectedValues(graph).Sum();
}