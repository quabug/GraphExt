using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

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

        public static IsEdgeCompatibleFunc CreateIsCompatibleFunc<TNode>([NotNull] GraphRuntime<TNode> graph, IReadOnlyDictionary<PortId, PortData> ports)
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

        public static GraphView.FindCompatiblePorts CreateFindCompatiblePortsFunc(
            [NotNull] IReadOnlyDictionary<Port, PortId> ports,
            [NotNull] IsEdgeCompatibleFunc isEdgeCompatibleFunc
        )
        {
            return FindPorts;

            IEnumerable<Port> FindPorts(Port startPort)
            {
                foreach (var portPair in ports)
                {
                    var endPort = portPair.Key;
                    if (startPort.orientation != endPort.orientation || startPort.direction == endPort.direction) continue;
                    var endPortId = portPair.Value;
                    var startPortId = ports[startPort];
                    if (!isEdgeCompatibleFunc(input: endPortId, output: startPortId)) continue;
                    yield return endPort;
                }
            }
        }
    }
}