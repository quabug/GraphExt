using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public interface IGraph
    {
        [NotNull] IEnumerable<PortData> Ports { get; }
        [NotNull] IEnumerable<INodeData> Nodes { get; }
        [NotNull] IEnumerable<EdgeId> Edges { get; }

        void DeleteNode(NodeId nodeId);
        void SetNodePosition(NodeId nodeId, Vector2 position);

        bool IsCompatible(PortId input, PortId output);
        void Connect(PortId input, PortId output);
        void Disconnect(PortId input, PortId output);
    }

    public abstract class Graph<TNode> : IGraph where TNode : INodeData
    {
        protected readonly Dictionary<NodeId, TNode> NodeMap;
        protected readonly Dictionary<PortId, PortData> PortMap;
        protected readonly Dictionary<PortId, ISet<EdgeId>> Connections;

        public IEnumerable<PortData> Ports => PortMap.Values;
        public IEnumerable<INodeData> Nodes => NodeMap.Values.Cast<INodeData>();
        public IEnumerable<EdgeId> Edges => Connections.Values.SelectMany(edges => edges);

        public Graph()
        {
            NodeMap = new Dictionary<NodeId, TNode>();
            PortMap = new Dictionary<PortId, PortData>();
            Connections = new Dictionary<PortId, ISet<EdgeId>>();
        }

        public Graph(IEnumerable<TNode> nodeList, IEnumerable<EdgeId> edges)
        {
            NodeMap = nodeList.ToDictionary(n => n.Id, n => n);
            PortMap = NodeMap.SelectMany(keyValue => keyValue.Value.Ports).ToDictionary(port => port.Id, port => port);
            Connections = new Dictionary<PortId, ISet<EdgeId>>();
            foreach (var edge in edges) AddConnection(edge);
        }

        public PortData this[PortId portId] => PortMap[portId];
        public TNode this[NodeId nodeId] => NodeMap[nodeId];

        public void DeleteNode(NodeId nodeId)
        {
            if (NodeMap.TryGetValue(nodeId, out var node))
            {
                NodeMap.Remove(nodeId);
                // TODO: remove ports and edges
            }
        }

        public void SetNodePosition(NodeId nodeId, Vector2 position)
        {
            if (NodeMap.TryGetValue(nodeId, out var node)) node.Position = position;
        }

        public bool IsCompatible(PortId inputPortId, PortId outputPortId)
        {
            var input = PortMap[inputPortId];
            var output = PortMap[outputPortId];
            return IsCompatible(input, output);
        }

        public void Connect(PortId input, PortId output)
        {
            AddConnection(new EdgeId(input, output));
            OnConnected(PortMap[input], PortMap[output]);
        }

        public void Disconnect(PortId input, PortId output)
        {
            RemoveConnection(new EdgeId(input, output));
            OnDisconnected(PortMap[input], PortMap[output]);
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
            if (!Connections.TryGetValue(key, out var connectedSet))
            {
                connectedSet = new HashSet<EdgeId>();
                Connections.Add(key, connectedSet);
            }
            return connectedSet;
        }

        public TNode FindNodeByPort(in PortId portId)
        {
            return NodeMap[portId.NodeId];
        }

        [NotNull] public IEnumerable<PortId> FindConnectedPorts(PortId portId)
        {
            return Connections.TryGetValue(portId, out var connected) ?
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