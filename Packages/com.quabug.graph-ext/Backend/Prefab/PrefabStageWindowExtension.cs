#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
            _view.OnNodeSelected += OnNodeViewSelected;
            _view.OnNodeUnselected += OnNodeViewUnselected;
            _selectedNodes.Clear();
        }

        private void OnNodeViewSelected(in NodeId nodeId, Node node)
        {
            if (!_selectedNodes.Contains(nodeId) && _viewModule.GameObjectNodes.NodeObjectMap.ContainsKey(nodeId))
            {
                _selectedNodes.Add(nodeId);
                SelectLast();
            }
        }

        private void OnNodeViewUnselected(in NodeId nodeId, Node node)
        {
            if (!_selectedNodes.Contains(nodeId)) return;
            _selectedNodes.Remove(nodeId);
            SelectLast();
        }

        private void SelectLast()
        {
            var invalidNodes = new List<NodeId>();
            foreach (var nodeId in _selectedNodes)
            {
                if (_viewModule.GameObjectNodes.NodeObjectMap.TryGetValue(nodeId, out var node))
                {
                    if (Selection.activeGameObject != node.gameObject) Selection.activeGameObject = node.gameObject;
                }
                else
                {
                    invalidNodes.Add(nodeId);
                }
            }

            foreach (var removed in invalidNodes)
            {
                _selectedNodes.Remove(removed);
            }
        }
    }
}

#endif