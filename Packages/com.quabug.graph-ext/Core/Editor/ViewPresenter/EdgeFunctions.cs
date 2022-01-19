using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt.Editor
{
    public delegate bool IsEdgeCompatibleFunc(in PortId input, in PortId output);
    public delegate void EdgeConnectFunc(in PortId input, in PortId output);
    public delegate void EdgeDisconnectFunc(in PortId input, in PortId output);

    public static class EdgeFunctions
    {
        public static EdgeConnectFunc Connect<TNode>([NotNull] GraphRuntime<TNode> graph)
            where TNode : INode<GraphRuntime<TNode>>
        {
            return (in PortId input, in PortId output) => graph.Connect(input: input, output: output);
        }

        public static EdgeDisconnectFunc Disconnect<TNode>([NotNull] GraphRuntime<TNode> graph)
            where TNode : INode<GraphRuntime<TNode>>
        {
            return (in PortId input, in PortId output) => graph.Disconnect(input: input, output: output);
        }

        public static IsEdgeCompatibleFunc IsCompatible<TNode>([NotNull] GraphRuntime<TNode> graph, IReadOnlyDictionary<PortId, PortData> ports)
            where TNode : INode<GraphRuntime<TNode>>
        {
            return (in PortId input, in PortId output) =>
            {
                var inputPort = ports[input];
                var outputPort = ports[output];
                return // single port could be handled by Unity Graph
                    (inputPort.Capacity == 1 || CountConnections(input) < inputPort.Capacity) &&
                    (outputPort.Capacity == 1 || CountConnections(output) < outputPort.Capacity) &&
                    graph.GetNodeByPort(input).IsPortCompatible(graph, input, output) &&
                    graph.GetNodeByPort(output).IsPortCompatible(graph, input, output)
                ;
            };

            int CountConnections(PortId portId)
            {
                return graph.Edges.Count(edge => edge.Contains(portId));
            }
        }
    }
}