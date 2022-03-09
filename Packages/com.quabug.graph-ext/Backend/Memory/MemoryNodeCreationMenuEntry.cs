#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryNodeCreationMenuEntry<TNode> : NodeCreationMenuEntry<TNode>
        where TNode : INode<GraphRuntime<TNode>>
    {
        private readonly IReadOnlyDictionary<NodeId, Node> _nodes;

        public MemoryNodeCreationMenuEntry(
            GraphRuntime<TNode> graphRuntime,
            IReadOnlyDictionary<NodeId, Node> nodes
        ) : base(graphRuntime)
        {
            _nodes = nodes;
        }

        protected override NodeId CreateNode(Type nodeType, Vector2 position)
        {
            var nodeId = base.CreateNode(nodeType, position);
            _nodes[nodeId].SetPosition(position);
            return nodeId;
        }
    }
}

#endif