using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public interface IGraphBackend
    {
        [NotNull] IReadOnlyDictionary<PortId, PortData> PortMap { get; }
        [NotNull] IReadOnlyDictionary<NodeId, NodeData> NodeMap { get; }
        // TODO: IReadOnlySet or ImmutableSet?
        [NotNull] IEnumerable<EdgeId> Edges { get; }

        void DeleteNode(in NodeId nodeId);
        bool IsCompatible(in PortId input, in PortId output);
        void Connect(in PortId input, in PortId output);
        void Disconnect(in PortId input, in PortId output);
    }

    public abstract class BaseGraphBackend : IGraphBackend
    {
        protected readonly Dictionary<NodeId, NodeData> _NodeMap;
        protected readonly Dictionary<PortId, PortData> _PortMap;
        protected readonly ISet<EdgeId> _Connections;

        public IReadOnlyDictionary<NodeId, NodeData> NodeMap => _NodeMap;
        public IReadOnlyDictionary<PortId, PortData> PortMap => _PortMap;
        public ISet<EdgeId> Connections => _Connections;

        public IEnumerable<EdgeId> Edges => _Connections;

        public BaseGraphBackend()
        {
            _NodeMap = new Dictionary<NodeId, NodeData>();
            _PortMap = new Dictionary<PortId, PortData>();
            _Connections = new HashSet<EdgeId>();
        }

        public BaseGraphBackend(IEnumerable<(NodeId id, NodeData data)> nodes, IEnumerable<(PortId id, PortData data)> ports, IEnumerable<EdgeId> edges)
        {
            _NodeMap = nodes.ToDictionary(node => node.id, n => n.data);
            _PortMap = ports.ToDictionary(port => port.id, port => port.data);
            _Connections = new HashSet<EdgeId>(edges);
        }

        public PortData this[in PortId portId] => _PortMap[portId];
        public NodeData this[in NodeId nodeId] => _NodeMap[nodeId];

        public void DeleteNode(in NodeId nodeId)
        {
            if (_NodeMap.TryGetValue(nodeId, out var node))
            {
                _NodeMap.Remove(nodeId);
                RemoveNodePorts(nodeId);
                OnNodeDeleted(nodeId);
            }
        }

        private void RemoveNodePorts(in NodeId nodeId)
        {
            var removedPorts = new HashSet<PortId>();
            foreach (var port in PortMap.Keys)
            {
                if (port.NodeId == nodeId) removedPorts.Add(port);
            }

            foreach (var port in removedPorts)
            {
                _PortMap.Remove(port);
                RemovePortEdges(port);
                OnPortDeleted(port);
            }
        }

        private void RemovePortEdges(in PortId portId)
        {
            var removedEdges = new HashSet<EdgeId>();
            foreach (var edge in Edges)
            {
                if (edge.Contains(portId)) removedEdges.Add(edge);
            }

            foreach (var edge in removedEdges)
            {
                Connections.Remove(edge);
                OnEdgeDeleted(edge);
            }
        }

        public void Connect(in PortId input, in PortId output)
        {
            Connections.Add(new EdgeId(input, output));
            OnConnected(input, output);
        }

        public void Disconnect(in PortId input, in PortId output)
        {
            Connections.Remove(new EdgeId(input, output));
            OnDisconnected(input, output);
        }

        [NotNull] public ISet<PortId> FindConnectedPorts(PortId portId)
        {
            return new HashSet<PortId>(_Connections
                .Where(connection => connection.Contains(portId))
                .Select(connection => portId == connection.First ? connection.Second : connection.First)
            );
        }

        [NotNull] public IEnumerable<NodeId> FindConnectedNodes(in PortId portId)
        {
            return FindConnectedPorts(portId).Select(port => port.NodeId);
        }

        public virtual bool IsCompatible(in PortId input, in PortId output) => true;
        protected virtual void OnConnected(in PortId input, in PortId output) {}
        protected virtual void OnDisconnected(in PortId input, in PortId outputId) {}
        protected virtual void OnNodeDeleted(in NodeId node) {}
        protected virtual void OnPortDeleted(in PortId port) {}
        protected virtual void OnEdgeDeleted(in EdgeId edge) {}
    }
}