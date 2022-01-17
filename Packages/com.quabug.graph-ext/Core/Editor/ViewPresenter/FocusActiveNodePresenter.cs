using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Object = UnityEngine.Object;

namespace GraphExt.Editor
{
    public class FocusActiveNodePresenter<TNodeComponent> : IViewPresenter, IDisposable
        where TNodeComponent : Object
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
        [NotNull] private readonly Func<TNodeComponent, Node> _getNodeView;

        public FocusActiveNodePresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView graphView,
            [NotNull] Func<TNodeComponent, Node> getNodeView
        )
        {
            _graphView = graphView;
            _getNodeView = getNodeView;
            Selection.selectionChanged += OnSelectionChanged;
        }

        void OnSelectionChanged()
        {
            if (Selection.activeObject is TNodeComponent node)
            {
                var nodeView = _getNodeView(node);
                if (!nodeView.selected)
                {
                    nodeView.Select(_graphView, additive: false);
                    _graphView.FrameSelection();
                }
            }
        }

        public void Dispose()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
    }
}