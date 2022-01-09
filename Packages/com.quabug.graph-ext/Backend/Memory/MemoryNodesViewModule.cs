using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryNodesViewModule<TNode> : INodesViewModule where TNode : INode<GraphRuntime<TNode>>
    {
        [NotNull] private IReadOnlyViewModuleElements<NodeId, Vector2> _nodePositions;
        private readonly IReadOnlyGraphRuntime<TNode> _graphRuntime;

        public MemoryNodesViewModule(
            [NotNull] IReadOnlyGraphRuntime<TNode> graphRuntime,
            [NotNull] IReadOnlyViewModuleElements<NodeId, Vector2> nodePositions
        )
        {
            _nodePositions = nodePositions;
            _graphRuntime = graphRuntime;
        }

        private NodeData ToNodeData(in NodeId id)
        {
            var node = _graphRuntime[id];
            var position = _nodePositions[id];
            var properties = new NodePositionProperty(position.x, position.y).Yield()
                .Append(NodeTitleAttribute.CreateTitleProperty(node))
                .Concat(node.CreateProperties(id))
            ;
            var ports = _graphRuntime[id].FindPorts();
            return new NodeData(properties, ports);
        }

        public IReadOnlyDictionary<NodeId, NodeData> GetNodes()
        {
            return _graphRuntime.NodeMap.Keys.ToDictionary(id => id, id => ToNodeData(id));
        }
    }
}