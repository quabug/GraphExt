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
            foreach (var node in nodes)
            {
                if (currentNodes.Contains(node)) currentNodes.Remove(node);
                else DeleteAndWarning(node);
            }

            foreach (var removed in currentNodes) _nodes.Runtime.DeleteNode(removed.Id);

            void DeleteAndWarning(TComponent node)
            {
                GameObject.DestroyImmediate(node.gameObject);
                Debug.LogWarning("add new node in hierarchy is not supported yet.");
            }
        }
    }
}

#endif