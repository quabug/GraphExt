using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public sealed class GraphRuntime<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        public delegate void OnNodeAddedFunc(in NodeId id, TNode node);
        public delegate void OnNodeDeletedFunc(in NodeId id, TNode node);
        public delegate void OnEdgeConnectedFunc(in EdgeId edge);
        public delegate void OnEdgeDisconnectedFunc(in EdgeId edge);
        public delegate bool IsCompatibleFunc(in PortId input, in PortId output);

        public event OnNodeAddedFunc OnNodeAdded;
        public event OnNodeDeletedFunc OnNodeDeleted;
        public event OnEdgeConnectedFunc OnEdgeConnected;
        public event OnEdgeDisconnectedFunc OnEdgeDisconnected;
        private readonly IsCompatibleFunc _compatibleCheck;

        private readonly BiDictionary<NodeId, TNode> _nodeMap = new BiDictionary<NodeId, TNode>();
        private readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        [NotNull] public IReadOnlySet<EdgeId> Edges => _edges;
        [NotNull] public IEnumerable<TNode> Nodes => _nodeMap.Forward.Values;
        [NotNull] public IReadOnlyDictionary<NodeId, TNode> NodeMap => _nodeMap.Forward;
        [NotNull] public IReadOnlyDictionary<TNode, NodeId> NodeIdMap => _nodeMap.Reverse;

        [NotNull] public TNode this[in NodeId id] => _nodeMap[id];
        public NodeId this[[NotNull] TNode node] => _nodeMap.GetKey(node);

        public GraphRuntime()
        {
            _compatibleCheck = (in PortId input, in PortId output) => true;
        }

        public GraphRuntime([NotNull] IsCompatibleFunc compatibleCheck)
        {
            _compatibleCheck = compatibleCheck;
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
                _nodeMap.Remove(nodeId);
                OnNodeDeleted?.Invoke(nodeId, node);
            }
        }

        [NotNull] public TNode GetNodeByPort(in PortId port) => this[port.NodeId];

        public bool IsCompatible(in PortId input, in PortId output)
        {
            CheckPortNodeExistence(input);
            CheckPortNodeExistence(output);
            return _compatibleCheck(input, output) &&
                   GetNodeByPort(input).IsPortCompatible(this, input, output) &&
                   GetNodeByPort(output).IsPortCompatible(this, input, output);
        }

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
            _edges.Remove(edge);
            GetNodeByPort(input).OnDisconnected(this, input, output);
            GetNodeByPort(output).OnDisconnected(this, input, output);
            OnEdgeDisconnected?.Invoke(edge);
        }

        void CheckPortNodeExistence(in PortId portId)
        {
            if (!_nodeMap.ContainsKey(portId.NodeId)) throw new InvalidPortException(portId);
        }

        private void RemoveNodeEdges(in NodeId nodeId)
        {
            foreach (var (input, output) in _edges.ToArray())
            {
                if (input.NodeId == nodeId || output.NodeId == nodeId)
                {
                    Disconnect(input, output);
                }
            }
        }
    }

    public static class GraphRuntimeExtension
    {
        [NotNull] public static IEnumerable<PortId> FindConnectedPorts<TNode>([NotNull] this GraphRuntime<TNode> graph, [NotNull] TNode node, [NotNull] string port)
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