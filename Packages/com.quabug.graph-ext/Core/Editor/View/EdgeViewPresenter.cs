using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class EdgeViewPresenter : IViewPresenter, IDisposable
    {
        [NotNull] private readonly IEdgesViewModule _edgesViewModule;
        [NotNull] private readonly IEdgeConnectionViewModule _edgeConnectionViewModule;
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _view;
        [NotNull] private readonly IEdgeViewFactory _edgeViewFactory;
        [NotNull] private readonly IGraphElements<EdgeId, Edge> _edges;
        [NotNull] private readonly IReadOnlyGraphElements<PortId, Port> _ports;

        public EdgeViewPresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView view,
            [NotNull] IEdgeViewFactory edgeViewFactory,
            [NotNull] IEdgeConnectionViewModule edgeConnectionViewModule,
            [NotNull] IEdgesViewModule edgesViewModule,
            [NotNull] IGraphElements<EdgeId, Edge> edges,
            [NotNull] IReadOnlyGraphElements<PortId, Port> ports
        )
        {
            _view = view;
            _edgeViewFactory = edgeViewFactory;
            _edgeConnectionViewModule = edgeConnectionViewModule;
            _edgesViewModule = edgesViewModule;
            _edges = edges;
            _ports = ports;
            _view.graphViewChanged += OnGraphChanged;
        }

        public void Tick()
        {
            var (added, removed) = _edges.Ids.Diff(_edgesViewModule.GetEdges());

            foreach (var edge in added)
            {
                var (input, output) = edge;
                if (!_ports.Has(input) || !_ports.Has(output)) continue;
                var edgeView = _edgeViewFactory.CreateEdge(_ports[output], _ports[input]);
                _edges.Add(edge, edgeView);
                _view.AddElement(edgeView);
            }

            foreach (var edge in removed)
            {
                var edgeView = _edges[edge];
                _ports[edge.Input].Disconnect(edgeView);
                _ports[edge.Output].Disconnect(edgeView);
                _view.RemoveElement(edgeView);
            }
        }

        public void Dispose()
        {
            _view.graphViewChanged -= OnGraphChanged;
        }

        private GraphViewChange OnGraphChanged(GraphViewChange @event)
        {
            if (@event.elementsToRemove != null)
            {
                foreach (var edge in @event.elementsToRemove.OfType<Edge>())
                {
                    var input = _ports[edge.input];
                    var output = _ports[edge.output];
                    var edgeId = new EdgeId(input, output);
                    if (_edges.Has(edgeId))
                    {
                        _edges.Remove(edgeId);
                        _edgeConnectionViewModule.Disconnect(input: input, output: output);
                    }
                }
            }

            if (@event.edgesToCreate != null)
            {
                foreach (var edge in @event.edgesToCreate)
                {
                    if (!_edges.Has(edge))
                    {
                        var input = _ports[edge.input];
                        var output = _ports[edge.output];
                        var edgeId = new EdgeId(input: input, output: output);
                        _edges.Add(edgeId, edge);
                        _edgeConnectionViewModule.Connect(input: input, output: output);
                        _edgeViewFactory.AfterCreated(edge);
                    }
                }
            }

            return @event;
        }
    }
}