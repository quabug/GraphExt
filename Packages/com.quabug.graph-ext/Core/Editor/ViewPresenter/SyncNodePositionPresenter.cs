using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    /// <summary>
    /// set node position on node moved in `GraphView`
    /// </summary>
    public class SyncNodePositionPresenter : IViewPresenter, IDisposable
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
        [NotNull] private readonly IReadOnlyDictionary<Node, NodeId> _nodeViews;
        [NotNull] private readonly IDictionary<NodeId, Vector2> _nodePositions;

        public SyncNodePositionPresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView graphView,
            [NotNull] IReadOnlyDictionary<Node, NodeId> nodeViews,
            [NotNull] IDictionary<NodeId, Vector2> nodePositions
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
                    _nodePositions[nodeId] = position;
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