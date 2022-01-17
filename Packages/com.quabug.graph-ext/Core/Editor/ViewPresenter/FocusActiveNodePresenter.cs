using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Object = UnityEngine.Object;

namespace GraphExt.Editor
{
    /// <summary>
    /// Make `GraphView` focus on selected object of `Selection`
    /// </summary>
    /// <typeparam name="TNodeComponent">type of node object</typeparam>
    public class FocusActiveNodePresenter<TNodeComponent> : IViewPresenter, IDisposable
        where TNodeComponent : Object
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
        [NotNull] private readonly Func<TNodeComponent, Node> _getNodeView;
        [NotNull] private readonly Func<TNodeComponent> _getSelectedNode;

        public FocusActiveNodePresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView graphView,
            [NotNull] Func<TNodeComponent, Node> getNodeView,
            [NotNull] Func<TNodeComponent> getSelectedNode
        )
        {
            _graphView = graphView;
            _getNodeView = getNodeView;
            _getSelectedNode = getSelectedNode;
            Selection.selectionChanged += OnSelectionChanged;
        }

        void OnSelectionChanged()
        {
            var node = _getSelectedNode();
            if (node == null) return;

            var nodeView = _getNodeView(node);
            if (nodeView.selected) return;

            nodeView.Select(_graphView, additive: false);
            _graphView.FrameSelection();
        }

        public void Dispose()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
    }
}