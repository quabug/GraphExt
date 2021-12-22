using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public interface IGraph
    {
        [NotNull] IEnumerable<PortData> Ports { get; }
        [NotNull] IEnumerable<INodeData> Nodes { get; }
        [NotNull] IEnumerable<EdgeId> Edges { get; }

        void DeleteNode(NodeId nodeId);

        bool IsCompatible(PortId input, PortId output);
        void Connect(PortId input, PortId output);
        void Disconnect(PortId input, PortId output);
    }

    public abstract class Graph<TNode> : IGraph where TNode : INodeData
    {
        protected readonly Dictionary<NodeId, TNode> _NodeMap;
        protected readonly Dictionary<PortId, PortData> _PortMap;
        protected readonly Dictionary<PortId, ISet<EdgeId>> _Connections;

        public IReadOnlyDictionary<NodeId, TNode> NodeMap => _NodeMap;
        public IReadOnlyDictionary<PortId, PortData> PortMap => _PortMap;
        public IReadOnlyDictionary<PortId, ISet<EdgeId>> Connections => _Connections;

        public IEnumerable<PortData> Ports => _PortMap.Values;
        public IEnumerable<INodeData> Nodes => _NodeMap.Values.Cast<INodeData>();
        public IEnumerable<EdgeId> Edges => _Connections.Values.SelectMany(edges => edges);

        public Graph()
        {
            _NodeMap = new Dictionary<NodeId, TNode>();
            _PortMap = new Dictionary<PortId, PortData>();
            _Connections = new Dictionary<PortId, ISet<EdgeId>>();
        }

        public Graph(IEnumerable<TNode> nodeList, IEnumerable<EdgeId> edges)
        {
            _NodeMap = nodeList.ToDictionary(n => n.Id, n => n);
            _PortMap = _NodeMap.SelectMany(keyValue => keyValue.Value.Ports).ToDictionary(port => port.Id, port => port);
            _Connections = new Dictionary<PortId, ISet<EdgeId>>();
            foreach (var edge in edges) AddConnection(edge);
        }

        public PortData this[PortId portId] => _PortMap[portId];
        public TNode this[NodeId nodeId] => _NodeMap[nodeId];

        public void DeleteNode(NodeId nodeId)
        {
            if (_NodeMap.TryGetValue(nodeId, out var node))
            {
                _NodeMap.Remove(nodeId);
                // TODO: remove ports and edges
            }
        }

        public bool IsCompatible(PortId inputPortId, PortId outputPortId)
        {
            var input = _PortMap[inputPortId];
            var output = _PortMap[outputPortId];
            return IsCompatible(input, output);
        }

        public void Connect(PortId input, PortId output)
        {
            AddConnection(new EdgeId(input, output));
            OnConnected(_PortMap[input], _PortMap[output]);
        }

        public void Disconnect(PortId input, PortId output)
        {
            RemoveConnection(new EdgeId(input, output));
            OnDisconnected(_PortMap[input], _PortMap[output]);
        }

        private void AddConnection(in EdgeId edge)
        {
            GetOrCreateEdgeSet(edge.First).Add(edge);
            GetOrCreateEdgeSet(edge.Second).Add(edge);
        }

        private void RemoveConnection(in EdgeId edge)
        {
            GetOrCreateEdgeSet(edge.First).Remove(edge);
            GetOrCreateEdgeSet(edge.Second).Remove(edge);
        }

        ISet<EdgeId> GetOrCreateEdgeSet(in PortId key)
        {
            if (!_Connections.TryGetValue(key, out var connectedSet))
            {
                connectedSet = new HashSet<EdgeId>();
                _Connections.Add(key, connectedSet);
            }
            return connectedSet;
        }

        public TNode FindNodeByPort(in PortId portId)
        {
            return _NodeMap[portId.NodeId];
        }

        [NotNull] public IEnumerable<PortId> FindConnectedPorts(PortId portId)
        {
            return _Connections.TryGetValue(portId, out var connected) ?
                connected.Select(edge => edge.First == portId ? edge.Second : edge.First) :
                Enumerable.Empty<PortId>()
            ;
        }

        [NotNull] public IEnumerable<NodeId> FindConnectedNodes(PortId portId)
        {
            return FindConnectedPorts(portId).Select(port => port.NodeId);
        }

        public virtual bool IsCompatible(PortData input, PortData output) => true;
        public virtual void OnConnected(PortData input, PortData output) {}
        public virtual void OnDisconnected(PortData input, PortData output) {}
    }
}