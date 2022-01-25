using System.Collections.Generic;

namespace GraphExt
{
    public interface IGraphBackend<TNode, TNodeComponent> where TNode : INode<GraphRuntime<TNode>>
    {
        GraphRuntime<TNode> Runtime { get; }
        public IReadOnlyList<TNodeComponent> Nodes { get; }
        public IReadOnlyDictionary<NodeId, TNodeComponent> NodeObjectMap { get; }
        public IReadOnlyDictionary<TNodeComponent, NodeId> ObjectNodeMap { get; }
    }
}