#if UNITY_EDITOR

using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace GraphExt.Editor
{
    public class PrefabStageWindowExtension<TNode, TComponent> : IWindowExtension
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        private GraphView _view;
        private GameObjectHierarchyGraphViewModule<TNode, TComponent> _viewModule;

        private readonly HashSet<NodeId> _selectedNodes = new HashSet<NodeId>();

        public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
        {
            _view = view;
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            ResetGraphBackend(stage);
            PrefabStage.prefabStageOpened += ResetGraphBackend;
            PrefabStage.prefabStageClosing += ClearEditorView;
            Selection.selectionChanged += OnPrefabSelectionChanged;
        }

        public void OnClosed(GraphWindow window, GraphConfig config, GraphView view)
        {
            PrefabStage.prefabStageOpened -= ResetGraphBackend;
            PrefabStage.prefabStageClosing -= ClearEditorView;
            Selection.selectionChanged -= OnPrefabSelectionChanged;
            _view = null;
        }

        private void OnPrefabSelectionChanged()
        {
            var selected = Selection.activeGameObject == null ? null : Selection.activeGameObject.GetComponent<TComponent>();
            if (selected != null && _viewModule.GameObjectNodes.ObjectNodeMap.TryGetValue(selected, out var nodeId))
            {
                var node = _view[nodeId];
                if (!_view.selection.Contains(node))
                {
                    node.Select(_view, additive: false);
                    _view.FrameSelection();
                }
            }
        }

        private void ClearEditorView(PrefabStage closingStage)
        {
            var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
            // do NOT clear editor if current stage is not closing
            ResetGraphBackend(currentStage == closingStage ? null : currentStage);
        }

        private void ResetGraphBackend([CanBeNull] PrefabStage prefabStage)
        {
            _viewModule = prefabStage == null ?
                new GameObjectHierarchyGraphViewModule<TNode, TComponent>() :
                new GameObjectHierarchyGraphViewModule<TNode, TComponent>(prefabStage.prefabContentsRoot)
            ;
            if (_view.Module is IDisposable disposable) disposable.Dispose();
            _view.Module = _viewModule;
            _viewModule.OnNodeSelectedChanged += OnNodeViewSelected;
            _selectedNodes.Clear();
        }

        private void OnNodeViewSelected(in NodeId node, bool isSelected)
        {
            if (isSelected)
            {
                if (_selectedNodes.Contains(node)) return;
                _selectedNodes.Add(node);
                Select(_viewModule.GameObjectNodes[node].gameObject);
            }
            else
            {
                if (!_selectedNodes.Contains(node)) return;
                _selectedNodes.Remove(node);
                Select(_selectedNodes.Any() ? _viewModule.GameObjectNodes[_selectedNodes.First()].gameObject : null);
            }
        }

        private void Select(GameObject node)
        {
            if (Selection.activeGameObject != node) Selection.activeGameObject = node;
        }
    }
}

#endif