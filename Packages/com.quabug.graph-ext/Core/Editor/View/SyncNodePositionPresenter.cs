using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class SyncNodePositionPresenter : IViewPresenter, IDisposable
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
        [NotNull] private readonly IReadOnlyGraphElements<NodeId, Node> _nodeViews;
        [NotNull] private readonly ViewModuleElements<NodeId, Vector2> _nodePositions;

        public SyncNodePositionPresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView graphView,
            [NotNull] IReadOnlyGraphElements<NodeId, Node> nodeViews,
            [NotNull] ViewModuleElements<NodeId, Vector2> nodePositions
        )
        {
            _graphView = graphView;
            _nodeViews = nodeViews;
            _nodePositions = nodePositions;
            _graphView.graphViewChanged += OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange evt)
        {
            if (evt.movedElements != null)
            {
                foreach (var nodeView in evt.movedElements.OfType<Node>())
                {
                    var nodeId = _nodeViews[nodeView];
                    var position = nodeView.GetVector2Position();
                    _nodePositions.Value[nodeId] = position;
                }
            }
            return evt;
        }

        public void Dispose()
        {
            _graphView.graphViewChanged -= OnGraphViewChanged;
        }
    }
}