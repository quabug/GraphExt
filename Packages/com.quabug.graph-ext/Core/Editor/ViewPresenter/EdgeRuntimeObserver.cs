using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class EdgeRuntimeObserver<TNode> : IWindowSystem, IDisposable
        where TNode : INode<GraphRuntime<TNode>>
    {
        private readonly GraphRuntime<TNode> _graph;
        private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
        private readonly IBiDictionary<EdgeId, Edge> _currentEdgeViews;
        private readonly IReadOnlyDictionary<PortId, Port> _ports;
        private readonly IEdgeViewFactory _edgeViewFactory;

        public EdgeRuntimeObserver(
            GraphRuntime<TNode> graph,
            UnityEditor.Experimental.GraphView.GraphView graphView,
            IBiDictionary<EdgeId, Edge> currentEdgeViews,
            IReadOnlyDictionary<PortId, Port> ports,
            IEdgeViewFactory edgeViewFactory
        )
        {
            _graph = graph;
            _graphView = graphView;
            _currentEdgeViews = currentEdgeViews;
            _ports = ports;
            _edgeViewFactory = edgeViewFactory;

            graph.OnEdgeConnected += OnEdgeConnected;
            graph.OnEdgeWillDisconnect += OnEdgeDisconnected;
        }

        private void OnEdgeConnected(in EdgeId edge)
        {
            if (_currentEdgeViews.ContainsKey(edge)) return;
            var inputPort = _ports[edge.Input];
            var outputPort = _ports[edge.Output];
            var edgeView = _edgeViewFactory.CreateEdge(inputPort, outputPort);
            _currentEdgeViews.Add(edge, edgeView);
            _graphView.AddElement(edgeView);
        }

        private void OnEdgeDisconnected(in EdgeId edge)
        {
            if (!_currentEdgeViews.ContainsKey(edge)) return;
            var edgeView = _currentEdgeViews[edge];
            _currentEdgeViews.Remove(edge);
            _graphView.DeleteElements(edgeView.Yield());
        }

        public void Dispose()
        {
            _graph.OnEdgeConnected -= OnEdgeConnected;
            _graph.OnEdgeWillDisconnect -= OnEdgeDisconnected;
        }
    }
}