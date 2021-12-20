using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public interface IGraphModule
    {
        [NotNull] IEnumerable<PortData> Ports { get; }
        [NotNull] IEnumerable<INodeModule> Nodes { get; }
        [NotNull] IEnumerable<EdgeId> Edges { get; }

        void DeleteNode(NodeId nodeId);
        void SetNodePosition(NodeId nodeId, Vector2 position);

        bool IsCompatible(PortId input, PortId output);
        void Connect(PortId input, PortId output);
        void Disconnect(PortId input, PortId output);
    }

    public abstract class GraphModule<TNode> : IGraphModule where TNode : INodeModule
    {
        protected readonly Dictionary<NodeId, TNode> NodeMap;
        protected readonly Dictionary<PortId, PortData> PortMap = new Dictionary<PortId, PortData>();
        protected readonly Dictionary<PortId, ISet<PortId>> Connections = new Dictionary<PortId, ISet<PortId>>();

        public IEnumerable<PortData> Ports => PortMap.Values;
        public IEnumerable<INodeModule> Nodes => NodeMap.Values.Cast<INodeModule>();

        private readonly ISet<EdgeId> _edgeCache = new HashSet<EdgeId>();
        public IEnumerable<EdgeId> Edges => _edgeCache;

        public GraphModule()
        {
            NodeMap = new Dictionary<NodeId, TNode>();
        }

        public GraphModule(IEnumerable<TNode> nodeList)
        {
            NodeMap = nodeList.ToDictionary(n => n.Id, n => n);
            PortMap = NodeMap.SelectMany(keyValue => keyValue.Value.Ports).ToDictionary(port => port.Id, port => port);
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
            AddConnection(input, output);
            AddConnection(output, input);
            _edgeCache.Add(new EdgeId(input, output));
            OnConnected(PortMap[input], PortMap[output]);
        }

        public void Disconnect(PortId input, PortId output)
        {
            RemoveConnection(input, output);
            RemoveConnection(output, input);
            _edgeCache.Remove(new EdgeId(input, output));
            OnDisconnected(PortMap[input], PortMap[output]);
        }

        private void AddConnection(PortId key, PortId value)
        {
            if (!Connections.TryGetValue(key, out var connectedSet))
            {
                connectedSet = new HashSet<PortId>();
                Connections.Add(key, connectedSet);
            }

            if (!connectedSet.Contains(value)) connectedSet.Add(value);
        }

        private void RemoveConnection(PortId key, PortId value)
        {
            if (Connections.TryGetValue(key, out var connectedSet))
                connectedSet.Remove(value);
        }

        public TNode FindNodeByPort(PortId portId)
        {
            return NodeMap[portId.NodeId];
        }

        [NotNull] public IEnumerable<PortId> FindConnectedPorts(PortId portId)
        {
            return Connections.TryGetValue(portId, out var connected) ? connected : Enumerable.Empty<PortId>();
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