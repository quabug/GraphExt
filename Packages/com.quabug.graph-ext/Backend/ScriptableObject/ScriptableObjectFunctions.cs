using System.Collections.Generic;
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
                var properties = NodePropertyUtility.CreateProperties(node, nodeId);
                var ports = NodePortUtility.FindPorts(node.Node);
                return new NodeData(properties, ports);
            };
        }
    }
}