using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public sealed class GraphRuntime<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        private readonly BiDictionary<NodeId, TNode> _nodeMap = new BiDictionary<NodeId, TNode>();
        private readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        public IReadOnlySet<EdgeId> Edges => _edges;
        public IEnumerable<TNode> Nodes => _nodeMap.Forward.Values;
        public IReadOnlyDictionary<NodeId, TNode> NodeMap => _nodeMap.Forward;
        public IReadOnlyDictionary<TNode, NodeId> NodeIdMap => _nodeMap.Reverse;

        public TNode this[in NodeId id] => _nodeMap[id];
        public NodeId this[in TNode node] => _nodeMap.GetKey(node);

        public void AddNode(in NodeId id, TNode node)
        {
            _nodeMap[id] = node;
        }

        public void DeleteNode(in NodeId nodeId)
        {
            if (_nodeMap.ContainsKey(nodeId))
            {
                _nodeMap.Remove(nodeId);
                RemoveNodeEdges(nodeId);
            }
        }

        public TNode GetNodeByPort(in PortId port) => this[port.NodeId];

        public bool IsCompatible(in PortId input, in PortId output)
        {
            return GetNodeByPort(input).IsPortCompatible(this, output, input) &&
                   GetNodeByPort(output).IsPortCompatible(this, output, input);
        }

        public void Connect(in PortId input, in PortId output)
        {
            _edges.Add(new EdgeId(input, output));
            GetNodeByPort(input).OnConnected(this, output, input);
            GetNodeByPort(output).OnConnected(this, output, input);
        }

        public void Disconnect(in PortId input, in PortId output)
        {
            _edges.Remove(new EdgeId(input, output));
            GetNodeByPort(input).OnDisconnected(this, output, input);
            GetNodeByPort(output).OnDisconnected(this, output, input);
        }
        private void RemoveNodeEdges(NodeId nodeId)
        {
            _edges.RemoveWhere(edge => edge.Input.NodeId == nodeId || edge.Output.NodeId == nodeId);
        }
    }

    public static class GraphRuntimeExtension
    {
        [NotNull] public static IEnumerable<PortId> FindConnectedPorts<TNode>([NotNull] this GraphRuntime<TNode> graph, [NotNull] TNode node, string port)
            where TNode : INode<GraphRuntime<TNode>>
        {
            return graph.FindConnectedPorts(new PortId(graph[node], port));
        }

        [NotNull] public static IEnumerable<PortId> FindConnectedPorts<TNode>([NotNull] this GraphRuntime<TNode> graph, PortId portId)
            where TNode : INode<GraphRuntime<TNode>>
        {
            return graph.Edges.SelectMany(edge => edge.GetConnectedPort(portId));
        }

        [NotNull] public static IEnumerable<NodeId> FindConnectedNodes<TNode>([NotNull] this GraphRuntime<TNode> graph, in PortId portId)
            where TNode : INode<GraphRuntime<TNode>>
        {
            return graph.FindConnectedPorts(portId).Select(port => port.NodeId);
        }

    }
}