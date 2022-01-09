using System.Linq;
using JetBrains.Annotations;

namespace GraphExt.Editor
{
    public interface IEdgeConnectionViewModule : IElementViewModule
    {
        void Connect(in PortId input, in PortId output);
        void Disconnect(in PortId input, in PortId output);
        bool IsCompatible(in PortId input, in PortId output);
    }

    public class EdgeConnectionViewModule<TNode> : IEdgeConnectionViewModule where TNode : INode<GraphRuntime<TNode>>
    {
        [NotNull] private readonly GraphRuntime<TNode> _graph;
        [NotNull] private readonly IReadOnlyViewModuleElements<PortId, PortData> _ports;

        public EdgeConnectionViewModule(
            [NotNull] GraphRuntime<TNode> graph,
            [NotNull] IReadOnlyViewModuleElements<PortId, PortData> ports
        )
        {
            _graph = graph;
            _ports = ports;
        }

        public void Connect(in PortId input, in PortId output)
        {
            _graph.Connect(input: input, output: output);
        }

        public void Disconnect(in PortId input, in PortId output)
        {
            _graph.Disconnect(input: input, output: output);
        }

        public bool IsCompatible(in PortId input, in PortId output)
        {
            var inputPort = _ports[input];
            var outputPort = _ports[output];
            return // single port could be handled by Unity Graph
                (inputPort.Capacity == 1 || CountConnections(input) < inputPort.Capacity) &&
                (outputPort.Capacity == 1 || CountConnections(output) < outputPort.Capacity) &&
                _graph.GetNodeByPort(input).IsPortCompatible(_graph, input, output) &&
                _graph.GetNodeByPort(output).IsPortCompatible(_graph, input, output)
                ;
        }

        private int CountConnections(PortId portId)
        {
            return _graph.Edges.Count(edge => edge.Contains(portId));
        }
    }
}