using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryNodesViewModule<TNode> : INodesViewModule where TNode : INode<GraphRuntime<TNode>>
    {
        [NotNull] private readonly IViewModuleElements<NodeId, Vector2> _nodePositions;
        [NotNull] private readonly IViewModuleElements<NodeId, NodeData> _nodes;
        [NotNull] private readonly IViewModuleElements<PortId, PortData> _ports;
        [NotNull] private readonly IReadOnlyGraphRuntime<TNode> _graphRuntime;

        public MemoryNodesViewModule(
            [NotNull] IReadOnlyGraphRuntime<TNode> graphRuntime,
            [NotNull] IViewModuleElements<NodeId, Vector2> nodePositions,
            [NotNull] IViewModuleElements<NodeId, NodeData> nodes,
            [NotNull] IViewModuleElements<PortId, PortData> ports
        )
        {
            _graphRuntime = graphRuntime;
            _nodePositions = nodePositions;
            _nodes = nodes;
            _ports = ports;
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

        public IEnumerable<(NodeId id, NodeData data)> GetNodes()
        {
            var (added, removed) = _nodes.Ids.Diff(_graphRuntime.NodeMap.Keys);

            foreach (var node in added)
            {
                if (!_nodePositions.Has(node)) _nodePositions[node] = Vector2.zero;
                var nodeData = ToNodeData(node);
                _nodes.Add(node, ToNodeData(node));
                foreach (var port in nodeData.Ports.Values) _ports.Add(new PortId(node, port.Name), port);
            }

            foreach (var node in removed)
            {
                var nodeData = _nodes[node];
                foreach (var port in nodeData.Ports.Values) _ports.Remove(new PortId(node, port.Name));
                _nodes.Remove(node);
                _nodePositions.Remove(node);
            }

            return _nodes.Elements;
        }
    }
}