﻿using JetBrains.Annotations;

namespace GraphExt
{
    public interface ITreeNode<in TGraph> : INode<TGraph>
    {
        string InputPortName { get; }
        string OutputPortName { get; }
    }

    public static partial class TreeNodeExtension
    {
        public static bool IsTreePort<TNode>([NotNull] this GraphRuntime<TNode> graph, in PortId port) where TNode : ITreeNode<GraphRuntime<TNode>>
        {
            return port.Name == graph[port.NodeId].InputPortName || port.Name == graph[port.NodeId].OutputPortName;
        }

        public static bool IsTreeEdge<TNode>([NotNull] this GraphRuntime<TNode> graph, in EdgeId edge) where TNode : ITreeNode<GraphRuntime<TNode>>
        {
            return graph.IsTreePort(edge.Input) && graph.IsTreePort(edge.Output);
        }
    }
}