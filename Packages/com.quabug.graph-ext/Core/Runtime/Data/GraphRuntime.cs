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
                _nodeMap.Remove(nodeId);
                RemoveNodeEdges(nodeId);
                OnNodeDeleted?.Invoke(nodeId, node);
            }
        }

        [NotNull] public TNode GetNodeByPort(in PortId port) => this[port.NodeId];

        public bool IsCompatible(in PortId input, in PortId output)
        {
            return _compatibleCheck(input, output) &&
                   GetNodeByPort(input).IsPortCompatible(this, output, input) &&
                   GetNodeByPort(output).IsPortCompatible(this, output, input);
        }

        public void Connect(in PortId input, in PortId output)
        {
            var edge = new EdgeId(input, output);
            if (!_edges.Contains(edge))
            {
                _edges.Add(edge);
                GetNodeByPort(input).OnConnected(this, output, input);
                GetNodeByPort(output).OnConnected(this, output, input);
                OnEdgeConnected?.Invoke(edge);
            }
        }

        public void Disconnect(in PortId input, in PortId output)
        {
            var edge = new EdgeId(input, output);
            if (_edges.Contains(edge))
            {
                _edges.Remove(edge);
                GetNodeByPort(input).OnDisconnected(this, output, input);
                GetNodeByPort(output).OnDisconnected(this, output, input);
                OnEdgeDisconnected?.Invoke(edge);
            }
        }

        private void RemoveNodeEdges(NodeId nodeId)
        {
            _edges.RemoveWhere(edge => edge.Input.NodeId == nodeId || edge.Output.NodeId == nodeId);
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