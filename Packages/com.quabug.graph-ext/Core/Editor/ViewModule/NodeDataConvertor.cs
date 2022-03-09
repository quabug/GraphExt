using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    public delegate NodeData ConvertToNodeData(in NodeId node);
    public delegate IEnumerable<PortData> FindPortData(in NodeId node);

    public static class NodeDataConvertor
    {
        public static ConvertToNodeData ToNodeData<TNode>(
            [NotNull] IReadOnlyDictionary<NodeId, TNode> nodes,
            IReadOnlyDictionary<NodeId, Vector2> nodePositions = null
        ) where TNode: INode<GraphRuntime<TNode>>
        {
            if (nodePositions == null) nodePositions = new Dictionary<NodeId, Vector2>();
            return (in NodeId nodeId) =>
            {
                var node = nodes[nodeId];
                nodePositions.TryGetValue(nodeId, out var position);
                var properties = new NodePositionProperty(position.x, position.y).Yield()
                    .Concat(NodePropertyUtility.CreateProperties(node, nodeId))
                ;
                return new NodeData(properties);
            };
        }

        public static ConvertToNodeData ToNodeData<TNodeComponent>(
            [NotNull] Func<NodeId, TNodeComponent> getNode,
            Func<NodeId, SerializedObject> getSerializedObject = null
        )
        {
            return (in NodeId nodeId) =>
            {
                var node = getNode(nodeId);
                var serializedNode = getSerializedObject?.Invoke(nodeId);
                var properties = NodePropertyUtility.CreateProperties(node, nodeId, name => serializedNode?.FindProperty(name));
                return new NodeData(properties);
            };
        }
    }

    public static class PortDataConvertor
    {
        public static FindPortData FindPorts<TNode>([NotNull] IReadOnlyDictionary<NodeId, TNode> nodes)
        {
            return (in NodeId nodeId) =>
            {
                var node = nodes[nodeId];
                return NodePortUtility.FindPorts(node);
            };
        }
    }
}