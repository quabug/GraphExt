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
                return new NodeData(properties);
            };
        }

        public static ConvertToNodeData ToNodeData<TNodeComponent>(
            [NotNull] IReadOnlyDictionary<NodeId, TNodeComponent> nodes,
            SerializedProperty nodeProperty = null
        )
        {
            return (in NodeId nodeId) =>
            {
                var node = nodes[nodeId];
                var properties = NodePropertyUtility.CreateProperties(node, nodeId, nodeProperty);
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