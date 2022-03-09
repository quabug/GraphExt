#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphExt.Editor
{
    public class ScriptableObjectNodeCreationMenuEntry<TNode, TNodeComponent> : NodeCreationMenuEntry<TNode>
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeComponent: NodeScriptableObject
    {
        private readonly IReadOnlyDictionary<NodeId, TNodeComponent> _nodes;

        public ScriptableObjectNodeCreationMenuEntry(
            GraphRuntime<TNode> graphRuntime,
            IReadOnlyDictionary<NodeId, TNodeComponent> nodes
        ) : base(graphRuntime)
        {
            _nodes = nodes;
        }

        protected override NodeId CreateNode(Type nodeType, Vector2 position)
        {
            var nodeId = base.CreateNode(nodeType, position);
            _nodes[nodeId].Position = position;
            return nodeId;
        }
    }
}

#endif