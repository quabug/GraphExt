using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryNodesViewModule<TNode> : INodesViewModule where TNode : INode<GraphRuntime<TNode>>
    {
        private readonly GraphRuntime<TNode> _graphRuntime;

        private readonly Dictionary<NodeId, Vector2> _nodePositions = new Dictionary<NodeId, Vector2>();

        private readonly Dictionary<NodeId, IReadOnlyDictionary<string, PortData>> _portsCache =
            new Dictionary<NodeId, IReadOnlyDictionary<string, PortData>>();

        public MemoryNodesViewModule(GraphRuntime<TNode> graphRuntime)
        {
            _graphRuntime = graphRuntime;
        }

        private IReadOnlyDictionary<string, PortData> FindNodePorts(in NodeId nodeId)
        {
            if (!_portsCache.TryGetValue(nodeId, out var ports))
            {
                ports = _graphRuntime[nodeId].FindPorts().ToDictionary(port => port.Name, port => port);
                _portsCache[nodeId] = ports;
            }
            return ports;
        }

        private NodeData ToNodeData(in NodeId id)
        {
            var node = _graphRuntime[id];
            var position = _nodePositions[id];
            return new NodeData(new NodePositionProperty(position.x, position.y).Yield()
                    .Append(NodeTitleAttribute.CreateTitleProperty(node))
                    .Concat(node.CreateProperties(id))
                    .ToArray()
            );
        }
    }
}