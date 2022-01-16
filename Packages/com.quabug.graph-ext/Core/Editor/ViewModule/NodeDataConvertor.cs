using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Editor
{

    public delegate NodeData ConvertToNodeData(in NodeId node);

    public static class NodeDataConvertor
    {
        public static ConvertToNodeData ToNodeData<TNode>(
            [NotNull] IReadOnlyDictionary<NodeId, TNode> nodes,
            [NotNull] IReadOnlyDictionary<NodeId, Vector2> nodePositions
        ) where TNode: INode<GraphRuntime<TNode>>
        {
            return (in NodeId nodeId) =>
            {
                var node = nodes[nodeId];
                var position = nodePositions[nodeId];
                var properties = new NodePositionProperty(position.x, position.y).Yield()
                    .Concat(NodePropertyUtility.CreateProperties(node, nodeId))
                ;
                var ports = NodePortUtility.FindPorts(nodes[nodeId]);
                return new NodeData(properties, ports);
            };
        }
    }
}