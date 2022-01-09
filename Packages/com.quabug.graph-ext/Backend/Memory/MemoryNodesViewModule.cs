using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryNodesViewModule<TNode> : INodesViewModule where TNode : INode<GraphRuntime<TNode>>
    {
        [NotNull] private readonly IReadOnlyViewModuleElements<NodeId, Vector2> _nodePositions;
        [NotNull] private readonly ViewModuleElements<NodeId, NodeData> _nodes;
        [NotNull] private readonly ViewModuleElements<PortId, PortData> _ports;
        [NotNull] private readonly IReadOnlyGraphRuntime<TNode> _graphRuntime;

        public MemoryNodesViewModule(
            [NotNull] IReadOnlyGraphRuntime<TNode> graphRuntime,
            [NotNull] IReadOnlyViewModuleElements<NodeId, Vector2> nodePositions,
            [NotNull] ViewModuleElements<NodeId, NodeData> nodes,
            [NotNull] ViewModuleElements<PortId, PortData> ports
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

        public IReadOnlyDictionary<NodeId, NodeData> GetNodes()
        {
            var (added, removed) = _nodes.Value.Keys.Diff(_graphRuntime.NodeMap.Keys);

            foreach (var node in added)
            {
                var nodeData = ToNodeData(node);
                _nodes.Value.Add(node, ToNodeData(node));
                foreach (var port in nodeData.Ports.Values) _ports.Value.Add(new PortId(node, port.Name), port);
            }

            foreach (var node in removed)
            {
                var nodeData = _nodes[node];
                foreach (var port in nodeData.Ports.Values) _ports.Value.Remove(new PortId(node, port.Name));
                _nodes.Value.Remove(node);
            }

            return _nodes.Value;
        }
    }
}