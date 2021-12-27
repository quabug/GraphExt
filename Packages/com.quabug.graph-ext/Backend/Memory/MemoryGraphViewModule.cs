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

        public MemoryGraphViewModule() {}

        public MemoryGraphViewModule([NotNull] GraphRuntime<TNode> runtime, [NotNull] IReadOnlyDictionary<NodeId, Vector2> positions)
        {
            foreach (var pair in runtime.NodeMap) AddMemoryNode(pair.Key, pair.Value, positions[pair.Key]);
            foreach (var edge in runtime.Edges) Runtime.Connect(edge.Input, edge.Output);
        }

        public void AddMemoryNode(in NodeId nodeId, [NotNull] TNode node, Vector2 position)
        {
            SetPosition(nodeId, position);
            AddNode(nodeId, node);
        }

        public void SetPosition(in NodeId nodeId, Vector2 position)
        {
            _nodePositions[nodeId] = position;
        }

        public override void DeleteNode(in NodeId nodeId)
        {
            _nodePositions.Remove(nodeId);
            base.DeleteNode(nodeId);
        }

        protected override IEnumerable<PortData> FindNodePorts(TNode node)
        {
            return NodePortUtility.FindPorts(node.GetType());
        }

        protected override NodeData ToNodeData(in NodeId id, [NotNull] TNode node)
        {
            var nodeId = id;

            return new NodeData(CreatePositionProperty().Yield()
                    .Append(NodeTitleAttribute.CreateTitleProperty(node))
                    .Concat(NodePropertyUtility.CreateProperties(node, id))
                    .ToArray()
            );

            INodeProperty CreatePositionProperty()
            {
                return new NodePositionProperty(_nodePositions[nodeId], position => _nodePositions[nodeId] = position);
            }
        }
    }
}

#endif