using System.Collections.Generic;
using System.Linq;
using GraphExt.Editor;
using JetBrains.Annotations;

namespace GraphExt
{
    public static class ScriptableObjectFunctions
    {
        public static ConvertToNodeData ToNodeData<TNode, TNodeScriptableObject>(
            [NotNull] IReadOnlyDictionary<NodeId, TNodeScriptableObject> nodes
        )
            where TNode : INode<GraphRuntime<TNode>>
            where TNodeScriptableObject : NodeScriptableObject<TNode>
        {
            return (in NodeId nodeId) =>
            {
                var node = nodes[nodeId];
                var position = nodePositions[nodeId];
                var properties = new NodePositionProperty(position.x, position.y).Yield()
                    .Append(NodeTitleAttribute.CreateTitleProperty(node))
                    .Concat(node.CreateProperties(nodeId))
                ;
                var ports = nodes[nodeId].FindPorts();
                return new NodeData(properties, ports);
            };
        }
    }
}