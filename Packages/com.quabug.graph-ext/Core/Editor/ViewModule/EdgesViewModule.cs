using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphExt.Editor
{
    public interface IEdgesViewModule : IElementViewModule
    {
        IReadOnlySet<EdgeId> GetEdges();
    }

    public class EdgesViewModule<TNode> : IEdgesViewModule where TNode : INode<GraphRuntime<TNode>>
    {
        private readonly IReadOnlyGraphRuntime<TNode> _graph;
        public EdgesViewModule([NotNull] IReadOnlyGraphRuntime<TNode> graph) => _graph = graph;
        public IReadOnlySet<EdgeId> GetEdges() => _graph.Edges;
    }
}