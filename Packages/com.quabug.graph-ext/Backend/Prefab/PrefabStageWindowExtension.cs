#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class PrefabStageWindowExtension : IWindowExtension
    {
        private GraphView _view;
        private PrefabGraphBackend _backend;

        private readonly HashSet<NodeId> _selectedNodes = new HashSet<NodeId>();

        public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
        {
            _view = view;
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            ResetGraphBackend(stage);
            PrefabStage.prefabStageOpened += ResetGraphBackend;
            PrefabStage.prefabStageClosing += ClearEditorView;
            Selection.selectionChanged += OnSelectionChanged;
        }

        public void OnClosed(GraphWindow window, GraphConfig config, GraphView view)
        {
            PrefabStage.prefabStageOpened -= ResetGraphBackend;
            PrefabStage.prefabStageClosing -= ClearEditorView;
            Selection.selectionChanged -= OnSelectionChanged;
            _view = null;
        }

        private void OnSelectionChanged()
        {
            if (Selection.activeGameObject != null && _backend.ObjectNodeMap.TryGetValue(Selection.activeGameObject, out var nodeId))
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
            _backend = prefabStage == null ? new PrefabGraphBackend() : new PrefabGraphBackend(prefabStage.prefabContentsRoot);
            _view.Module = _backend;
            _backend.OnNodeSelectedChanged += OnNodeSelected;
            _selectedNodes.Clear();
        }

        private void OnNodeSelected(NodeId node, bool isSelected)
        {
            if (isSelected)
            {
                if (_selectedNodes.Contains(node)) return;
                _selectedNodes.Add(node);
                Select(_backend.NodeObjectMap[node]);
            }
            else
            {
                if (!_selectedNodes.Contains(node)) return;
                _selectedNodes.Remove(node);
                Select(_selectedNodes.Any() ? _backend.NodeObjectMap[_selectedNodes.First()] : null);
            }
        }

        private void Select(GameObject node)
        {
            if (Selection.activeGameObject != node) Selection.activeGameObject = node;
        }
    }
}

#endif