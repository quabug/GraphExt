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
    [NodePort(Hide = true, Capacity = 2, SerializeId = "out")] protected static readonly float[] _Output;
    [NodePort(Hide = true, SerializeId = "in")] protected static readonly float _Input;
    public string InputPortName => nameof(_Input);
    public string OutputPortName => nameof(_Output);

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
    [NodeProperty(InputPort = nameof(_Input))] public float Value;
    public override float GetValue(GraphRuntime<IVisualNode> graph) => Value;

    [NodePort(SerializeId = "out1")] protected static float _output1;
}

public class AddNode : VisualNode
{
    [NodeProperty(InputPort = nameof(_Input), OutputPort = nameof(_Output), HideValue = true, Name = "add")]
    private const int _ = 0;
    public override float GetValue(GraphRuntime<IVisualNode> graph) => GetConnectedValues(graph).Sum();

    [NodePort(SerializeId = "in1")] protected static float[] _input1 = new float[3];
}