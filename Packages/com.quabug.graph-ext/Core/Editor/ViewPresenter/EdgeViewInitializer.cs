using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    /// <summary>
    /// Create edges while initialize
    /// </summary>
    public class EdgeViewInitializer : IInitializableWindowSystem
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _view;
        [NotNull] private readonly IEdgeViewFactory _edgeViewFactory;
        private readonly IEnumerable<EdgeId> _edges;
        [NotNull] private readonly IBiDictionary<EdgeId, Edge> _edgeViews;
        [NotNull] private readonly IReadOnlyBiDictionary<PortId, Port> _portViews;

        public EdgeViewInitializer(
            UnityEditor.Experimental.GraphView.GraphView view,
            IEdgeViewFactory edgeViewFactory,
            IEnumerable<EdgeId> edges,
            IBiDictionary<EdgeId, Edge> edgeViews,
            IReadOnlyBiDictionary<PortId, Port> portViews
        )
        {
            _view = view;
            _edgeViewFactory = edgeViewFactory;
            _edges = edges;
            _edgeViews = edgeViews;
            _portViews = portViews;
        }

        public void Initialize()
        {
            foreach (var edge in _edges)
            {
                var (input, output) = edge;
                if (!_portViews.ContainsKey(input) || !_portViews.ContainsKey(output))
                {
                    Debug.LogError("port is not created yet");
                    continue;
                }
                var edgeView = _edgeViewFactory.CreateEdge(_portViews[output], _portViews[input]);
                _edgeViews.Add(edge, edgeView);
                _view.AddElement(edgeView);
            }
        }
    }
}