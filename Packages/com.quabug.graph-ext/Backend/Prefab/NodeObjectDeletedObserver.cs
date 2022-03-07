#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace GraphExt.Editor
{
    public class NodeObjectDeletedObserver<TNode, TComponent> : IWindowSystem, IDisposable
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        private readonly PrefabStage _prefabStage;
        private readonly GameObjectNodes<TNode, TComponent> _nodes;

        public NodeObjectDeletedObserver(PrefabStage prefabStage, GameObjectNodes<TNode, TComponent> nodes)
        {
            Assert.IsNotNull(prefabStage);
            Assert.IsNotNull(nodes);
            _prefabStage = prefabStage;
            _nodes = nodes;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        public void Dispose()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        }

        private void OnHierarchyChanged()
        {
            Assert.AreEqual(_prefabStage, PrefabStageUtility.GetCurrentPrefabStage());
            var graphNodes = new HashSet<TComponent>(_nodes.Nodes);
            var hierarchyNodes = new HashSet<TComponent>(_prefabStage.prefabContentsRoot.GetComponentsInChildren<TComponent>());
            graphNodes.ExceptWith(hierarchyNodes);
            foreach (var deleted in graphNodes) _nodes.Runtime.DeleteNode(deleted.Id);
        }
    }
}

#endif