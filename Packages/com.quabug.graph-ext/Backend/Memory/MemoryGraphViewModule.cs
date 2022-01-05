#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Editor
{
    public sealed class MemoryGraphViewModule<TNode> : GraphViewModule<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        public override GraphRuntime<TNode> Runtime { get; } = new GraphRuntime<TNode>();

        public IEnumerable<(NodeId id, Vector2 position)> NodePositions => _nodePositions.Select(pair => (pair.Key, pair.Value));
        private readonly Dictionary<NodeId, Vector2> _nodePositions = new Dictionary<NodeId, Vector2>();

        private readonly Dictionary<NodeId, IReadOnlyDictionary<string, PortData>> _portsCache =
            new Dictionary<NodeId, IReadOnlyDictionary<string, PortData>>();

        public MemoryGraphViewModule() {}

        public MemoryGraphViewModule([NotNull] GraphRuntime<TNode> runtime, [NotNull] IReadOnlyDictionary<NodeId, Vector2> positions)
        {
            foreach (var pair in runtime.NodeMap) AddMemoryNode(pair.Key, pair.Value, positions[pair.Key]);
            foreach (var edge in runtime.Edges) Connect(edge.Input, edge.Output);
        }

        public void AddMemoryNode(in NodeId nodeId, [NotNull] TNode node, Vector2 position)
        {
            AddNode(nodeId, node, position.x, position.y);
        }

        public override void SetNodePosition(in NodeId nodeId, float x, float y)
        {
            _nodePositions[nodeId] = new Vector2(x, y);
        }

        public override void DeleteNode(in NodeId nodeId)
        {
            _nodePositions.Remove(nodeId);
            base.DeleteNode(nodeId);
        }

        protected override IReadOnlyDictionary<string, PortData> FindNodePorts(in NodeId nodeId)
        {
            if (!_portsCache.TryGetValue(nodeId, out var ports))
            {
                ports = Runtime[nodeId].FindPorts().ToDictionary(port => port.Name, port => port);
                _portsCache[nodeId] = ports;
            }
            return ports;
        }

        protected override NodeData ToNodeData(in NodeId id)
        {
            var node = Runtime[id];
            var position = _nodePositions[id];
            return new NodeData(new NodePositionProperty(position.x, position.y).Yield()
                    .Append(NodeTitleAttribute.CreateTitleProperty(node))
                    .Concat(node.CreateProperties(id))
                    .ToArray()
            );
        }
    }
}

#endif