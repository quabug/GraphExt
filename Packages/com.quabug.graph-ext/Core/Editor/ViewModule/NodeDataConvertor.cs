using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    public delegate NodeData ConvertToNodeData(in NodeId node);

    public static class NodeDataConvertor
    {
        public static ConvertToNodeData ToNodeData<TNode>(
            [NotNull] IReadOnlyDictionary<NodeId, TNode> nodes,
            [NotNull] IReadOnlyDictionary<NodeId, Vector2> nodePositions,
            SerializedProperty nodeProperty = null
        ) where TNode: INode<GraphRuntime<TNode>>
        {
            return (in NodeId nodeId) =>
            {
                var node = nodes[nodeId];
                var position = nodePositions[nodeId];
                var properties = new NodePositionProperty(position.x, position.y).Yield()
                    .Concat(NodePropertyUtility.CreateProperties(node, nodeId, nodeProperty))
                ;
                var ports = NodePortUtility.FindPorts(nodes[nodeId]);
                return new NodeData(properties, ports);
            };
        }

        public static ConvertToNodeData ToNodeData<TNode, TNodeComponent>(
            [NotNull] IReadOnlyDictionary<NodeId, TNodeComponent> nodes,
            [NotNull] Func<TNodeComponent, TNode> getNode,
            SerializedProperty nodeProperty = null
        )
        {
            return (in NodeId nodeId) =>
            {
                var node = nodes[nodeId];
                var properties = NodePropertyUtility.CreateProperties(node, nodeId, nodeProperty);
                var ports = NodePortUtility.FindPorts(getNode(node));
                return new NodeData(properties, ports);
            };
        }

        public static ConvertToNodeData ToNodeData<TNodeComponent>(
            [NotNull] IReadOnlyDictionary<NodeId, TNodeComponent> nodes,
            [NotNull] Func<TNodeComponent, IEnumerable<PortData>> findPorts,
            SerializedProperty nodeProperty = null
        )
        {
            return (in NodeId nodeId) =>
            {
                var node = nodes[nodeId];
                var properties = NodePropertyUtility.CreateProperties(node, nodeId, nodeProperty);
                var ports = findPorts(node);
                return new NodeData(properties, ports);
            };
        }
    }
}