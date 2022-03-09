#if UNITY_EDITOR

using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    [UsedImplicitly]
    public class NodeHierarchyObserver<TNode, TComponent> : IWindowSystem, IDisposable
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        private readonly GameObjectNodes<TNode, TComponent> _nodes;

        public NodeHierarchyObserver(GameObjectNodes<TNode, TComponent> nodes)
        {
            _nodes = nodes;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        public void Dispose()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        }

        private void OnHierarchyChanged()
        {
            var currentNodes = new HashSet<TComponent>(_nodes.Nodes);
            var nodes = _nodes.Root.GetComponentsInChildren<TComponent>();
            var nodeIds = new HashSet<NodeId>(_nodes.NodeMap.Keys);
            foreach (var node in nodes)
            {
                if (currentNodes.Contains(node)) currentNodes.Remove(node);
                else AddNode(node);
            }

            foreach (var removed in currentNodes) _nodes.Runtime.DeleteNode(removed.Id);

            void AddNode(TComponent node)
            {
                if (node.Id == Guid.Empty || nodeIds.Contains(node.Id)) node.Id = Guid.NewGuid();
                nodeIds.Add(node.Id);
                _nodes.AddNode(node);
                _nodes.Runtime.AddNode(node.Id, node.Node);
                foreach (var edge in node.GetEdges(_nodes.Runtime)) _nodes.Runtime.Connect(edge.Input, edge.Output);
            }
        }
    }
}

#endif