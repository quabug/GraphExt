using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public delegate void OnNodeAddedFunc<in TNode>(in NodeId id, [NotNull] TNode node) where TNode : INode<GraphRuntime<TNode>>;
    public delegate void OnNodeWillDeleteFunc<in TNode>(in NodeId id, [NotNull] TNode node) where TNode : INode<GraphRuntime<TNode>>;
    public delegate void OnEdgeConnectedFunc(in EdgeId edge);
    public delegate void OnEdgeWillDisconnectFunc(in EdgeId edge);

    public interface IReadOnlyGraphRuntime<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        public event OnNodeAddedFunc<TNode> OnNodeAdded;
        public event OnNodeWillDeleteFunc<TNode> OnNodeWillDelete;
        public event OnEdgeConnectedFunc OnEdgeConnected;
        public event OnEdgeWillDisconnectFunc OnEdgeWillDisconnect;

        [NotNull] public IReadOnlySet<EdgeId> Edges { get; }
        [NotNull] public IReadOnlyDictionary<NodeId, TNode> IdNodeMap { get; }
        [NotNull] public IReadOnlyDictionary<TNode, NodeId> NodeIdMap { get; }

        [NotNull] public TNode this[in NodeId id] { get; }
        public NodeId this[[NotNull] TNode node] { get; }
    }

    /// <summary>
    /// Runtime graph data including nodes and edges
    /// </summary>
    /// <typeparam name="TNode">type of <seealso cref="INode{TGraph}"/></typeparam>
    public sealed class GraphRuntime<TNode> : IReadOnlyGraphRuntime<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        public event OnNodeAddedFunc<TNode> OnNodeAdded;
        public event OnNodeWillDeleteFunc<TNode> OnNodeWillDelete;
        public event OnEdgeConnectedFunc OnEdgeConnected;
        public event OnEdgeWillDisconnectFunc OnEdgeWillDisconnect;

        private readonly BiDictionary<NodeId, TNode> _nodeMap;
        private readonly HashSet<EdgeId> _edges;

        public IReadOnlySet<EdgeId> Edges => _edges;
        public IReadOnlyDictionary<NodeId, TNode> IdNodeMap => _nodeMap.Forward;
        public IReadOnlyDictionary<TNode, NodeId> NodeIdMap => _nodeMap.Reverse;
        public IReadOnlyBiDictionary<NodeId, TNode> NodeMap => _nodeMap;

        public TNode this[in NodeId id] => _nodeMap[id];
        public NodeId this[TNode node] => _nodeMap.GetKey(node);

        public IEnumerable<(NodeId, TNode)> Nodes => _nodeMap.Forward.Select(t => (t.Key, t.Value));

        public GraphRuntime()
        {
            _edges = new HashSet<EdgeId>();
            _nodeMap = new BiDictionary<NodeId, TNode>();
        }

        public GraphRuntime(IReadOnlyGraphRuntime<TNode> graphRuntime)
        {
            _edges = graphRuntime.Edges.ToHashSet();
            _nodeMap = new BiDictionary<NodeId, TNode>(graphRuntime.IdNodeMap);
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
            if (_edges.Contains(edge)) throw new EdgeAlreadyConnectedException(edge);
            _edges.Add(edge);
            if (_nodeMap.TryGetValue(edge.Input.NodeId, out var inputNode))
                inputNode.OnConnected(this, input, output);
            if (_nodeMap.TryGetValue(edge.Output.NodeId, out var outputNode))
                outputNode.OnConnected(this, input, output);
            OnEdgeConnected?.Invoke(edge);
        }

        public void Disconnect(in PortId input, in PortId output)
        {
            var edge = new EdgeId(input, output);
            if (!_edges.Contains(edge)) throw new EdgeAlreadyDisconnectedException(edge);
            OnEdgeWillDisconnect?.Invoke(edge);
            _edges.Remove(edge);
            if (_nodeMap.TryGetValue(edge.Input.NodeId, out var inputNode))
                inputNode.OnDisconnected(this, input, output);
            if (_nodeMap.TryGetValue(edge.Output.NodeId, out var outputNode))
                outputNode.OnDisconnected(this, input, output);
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