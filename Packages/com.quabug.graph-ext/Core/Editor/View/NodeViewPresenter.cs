using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class NodeViewPresenter
    {
        [NotNull] private readonly IEdgesViewModule _viewModule;
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _view;
        [NotNull] private readonly GraphElements<EdgeId, Edge> _edges;
        [NotNull] private readonly GraphElements<PortId, Port> _ports;

        public NodeViewPresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView view,
            [NotNull] IEdgesViewModule viewModule,
            [NotNull] GraphElements<EdgeId, Edge> edges,
            [NotNull] GraphElements<PortId, Port> ports
        )
        {
            _view = view;
            _viewModule = viewModule;
            _edges = edges;
            _ports = ports;
        }

        public void Tick()
        {
            var (added, removed) = _edges.Ids.Diff(_viewModule.Edges);

            foreach (var edge in added)
            {
                var (input, output) = edge;
                if (!_ports.Has(input) || !_ports.Has(output)) continue;
                var edgeView = _ports[output].ConnectTo(_ports[input]);
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
    }
}