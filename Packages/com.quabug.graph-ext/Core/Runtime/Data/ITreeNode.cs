using JetBrains.Annotations;

namespace GraphExt
{
    public interface ITreeNode<in TGraph> : INode<TGraph>
    {
        string InputPortName { get; }
        string OutputPortName { get; }
    }

    public static class TreeNodeExtension
    {
        public static bool IsTreePort<TNode>([NotNull] this GraphRuntime<TNode> graph, in PortId port) where TNode : INode<GraphRuntime<TNode>>
        {
            return graph[port.NodeId] is ITreeNode<GraphRuntime<TNode>> node &&
                   (port.Name == node.InputPortName || port.Name == node.OutputPortName);
        }

        public static bool IsTreeEdge<TNode>([NotNull] this GraphRuntime<TNode> graph, in EdgeId edge) where TNode : ITreeNode<GraphRuntime<TNode>>
        {
            return graph.IsTreePort(edge.Input) && graph.IsTreePort(edge.Output);
        }
    }
}