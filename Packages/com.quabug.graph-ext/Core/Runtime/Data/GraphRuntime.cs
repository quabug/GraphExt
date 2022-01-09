using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public interface IReadOnlyGraphRuntime<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        public delegate void OnNodeAddedFunc(in NodeId id, [NotNull] TNode node);
        public delegate void OnNodeWillDeleteFunc(in NodeId id, [NotNull] TNode node);
        public delegate void OnEdgeConnectedFunc(in EdgeId edge);
        public delegate void OnEdgeWillDisconnectFunc(in EdgeId edge);

        public event OnNodeAddedFunc OnNodeAdded;
        public event OnNodeWillDeleteFunc OnNodeWillDelete;
        public event OnEdgeConnectedFunc OnEdgeConnected;
        public event OnEdgeWillDisconnectFunc OnEdgeWillDisconnect;

        [NotNull] public IReadOnlySet<EdgeId> Edges { get; }
        [NotNull] public IReadOnlyDictionary<NodeId, TNode> NodeMap { get; }
        [NotNull] public IReadOnlyDictionary<TNode, NodeId> NodeIdMap { get; }

        [NotNull] public TNode this[in NodeId id] { get; }
        public NodeId this[[NotNull] TNode node] { get; }
    }

    public sealed class GraphRuntime<TNode> : IReadOnlyGraphRuntime<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        public event IReadOnlyGraphRuntime<TNode>.OnNodeAddedFunc OnNodeAdded;
        public event IReadOnlyGraphRuntime<TNode>.OnNodeWillDeleteFunc OnNodeWillDelete;
        public event IReadOnlyGraphRuntime<TNode>.OnEdgeConnectedFunc OnEdgeConnected;
        public event IReadOnlyGraphRuntime<TNode>.OnEdgeWillDisconnectFunc OnEdgeWillDisconnect;

        private readonly BiDictionary<NodeId, TNode> _nodeMap;
        private readonly HashSet<EdgeId> _edges;

        public IReadOnlySet<EdgeId> Edges => _edges;
        public IReadOnlyDictionary<NodeId, TNode> NodeMap => _nodeMap.Forward;
        public IReadOnlyDictionary<TNode, NodeId> NodeIdMap => _nodeMap.Reverse;

        public TNode this[in NodeId id] => _nodeMap[id];
        public NodeId this[TNode node] => _nodeMap.GetKey(node);

        public GraphRuntime()
        {
            _edges = new HashSet<EdgeId>();
            _nodeMap = new BiDictionary<NodeId, TNode>();
        }

        public GraphRuntime(IReadOnlyGraphRuntime<TNode> graphRuntime)
        {
            _edges = graphRuntime.Edges.ToHashSet();
            _nodeMap = new BiDictionary<NodeId, TNode>(graphRuntime.NodeMap);
        }

        public void AddNode(in NodeId id, [NotNull] TNode node)
        {
            _nodeMap[id] = node;
            OnNodeAdded?.Invoke(id, node);
        }

        public void DeleteNode(in NodeId nodeId)
        {
            if (_nodeMap.TryGetValue(nodeId, out var node))
            {
                RemoveNodeEdges(nodeId);
                OnNodeWillDelete?.Invoke(nodeId, node);
                _nodeMap.Remove(nodeId);
            }
        }

        [NotNull] public TNode GetNodeByPort(in PortId port) => this[port.NodeId];

        public void Connect(in PortId input, in PortId output)
        {
            var edge = new EdgeId(input, output);
            CheckPortNodeExistence(input);
            CheckPortNodeExistence(output);
            if (_edges.Contains(edge)) throw new EdgeAlreadyConnectedException(edge);
            _edges.Add(edge);
            GetNodeByPort(input).OnConnected(this, input, output);
            GetNodeByPort(output).OnConnected(this, input, output);
            OnEdgeConnected?.Invoke(edge);
        }

        public void Disconnect(in PortId input, in PortId output)
        {
            var edge = new EdgeId(input, output);
            CheckPortNodeExistence(input);
            CheckPortNodeExistence(output);
            if (!_edges.Contains(edge)) throw new EdgeAlreadyDisconnectedException(edge);
            OnEdgeWillDisconnect?.Invoke(edge);
            _edges.Remove(edge);
            GetNodeByPort(input).OnDisconnected(this, input, output);
            GetNodeByPort(output).OnDisconnected(this, input, output);
        }

        void CheckPortNodeExistence(in PortId portId)
        {
            if (!_nodeMap.ContainsKey(portId.NodeId)) throw new InvalidPortException(portId);
        }

        private void RemoveNodeEdges(NodeId nodeId)
        {
            var removed = _edges.Where(edge => edge.Input.NodeId == nodeId || edge.Output.NodeId == nodeId).ToArray();
            foreach (var (input, output) in removed) Disconnect(input, output);
        }
    }

    public static class GraphRuntimeExtension
    {
        [NotNull] public static IEnumerable<PortId> FindConnectedPorts<TNode>([NotNull] this IReadOnlyGraphRuntime<TNode> graph, [NotNull] TNode node, [NotNull] string port)
            where TNode : INode<GraphRuntime<TNode>>
        {
            return graph.FindConnectedPorts(new PortId(graph[node], port));
        }

        [NotNull] public static IEnumerable<PortId> FindConnectedPorts<TNode>([NotNull] this IReadOnlyGraphRuntime<TNode> graph, PortId portId)
            where TNode : INode<GraphRuntime<TNode>>
        {
            return graph.Edges.SelectMany(edge => edge.GetConnectedPort(portId));
        }

        [NotNull] public static IEnumerable<NodeId> FindConnectedNodes<TNode>([NotNull] this IReadOnlyGraphRuntime<TNode> graph, in PortId portId)
            where TNode : INode<GraphRuntime<TNode>>
        {
            return graph.FindConnectedPorts(portId).Select(port => port.NodeId);
        }

    }
}