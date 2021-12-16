using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public interface IGraphModule
    {
        [NotNull] IEnumerable<INodeModule> Nodes { get; }
        [NotNull] IEnumerable<EdgeId> Edges { get; }

        void DeleteNode(Guid nodeId);
        void SetNodePosition(Guid nodeId, Vector2 position);

        bool IsCompatible([NotNull] IPortModule input, [NotNull] IPortModule output);
        void Connect([NotNull] IPortModule input, [NotNull] IPortModule output);
        void Disconnect([NotNull] IPortModule input, [NotNull] IPortModule output);
    }

    public abstract class GraphModule<TNode, TPort> : IGraphModule where TNode : INodeModule where TPort : IPortModule
    {
        protected readonly Dictionary<Guid, TNode> NodeMap;
        protected readonly Dictionary<Guid, TPort> PortMap = new Dictionary<Guid, TPort>();
        protected readonly Dictionary<TPort, ISet<TPort>> Connections = new Dictionary<TPort, ISet<TPort>>();

        public IEnumerable<INodeModule> Nodes => NodeMap.Values.Cast<INodeModule>();

        private readonly ISet<EdgeId> _edgeCache = new HashSet<EdgeId>();
        public IEnumerable<EdgeId> Edges => _edgeCache;

        public GraphModule()
        {
            NodeMap = new Dictionary<Guid, TNode>();
        }

        public GraphModule(IEnumerable<TNode> nodeList)
        {
            NodeMap = nodeList.ToDictionary(n => n.Id, n => n);
        }

        public void DeleteNode(Guid nodeId)
        {
            if (NodeMap.TryGetValue(nodeId, out var node))
            {
                NodeMap.Remove(nodeId);
                // TODO: remove ports and edges
            }
        }

        public void SetNodePosition(Guid nodeId, Vector2 position)
        {
            if (NodeMap.TryGetValue(nodeId, out var node)) node.Position = position;
        }

        public bool IsCompatible(IPortModule input, IPortModule output)
        {
            return input.Direction != output.Direction &&
                   input.PortType == output.PortType &&
                   input is TPort @in && output is TPort @out &&
                   IsCompatible(@in, @out)
            ;
        }

        public void Connect(IPortModule input, IPortModule output)
        {
            if (input is TPort @in && output is TPort @out)
            {
                AddConnection(@in, @out);
                AddConnection(@out, @in);
                _edgeCache.Add(new EdgeId(outputPort: output.Id, inputPort: input.Id));
                OnConnected(@in, @out);
            }
        }

        public void Disconnect(IPortModule input, IPortModule output)
        {
            if (input is TPort @in && output is TPort @out)
            {
                RemoveConnection(@in, @out);
                RemoveConnection(@out, @in);
                _edgeCache.Remove(new EdgeId(output.Id, input.Id));
                OnDisconnected(@in, @out);
            }
        }

        private void AddConnection(TPort key, TPort value)
        {
            if (!Connections.TryGetValue(key, out var connectedSet))
            {
                connectedSet = new HashSet<TPort>();
                Connections.Add(key, connectedSet);
            }

            if (!connectedSet.Contains(value)) connectedSet.Add(value);
        }

        private void RemoveConnection(TPort key, TPort value)
        {
            if (Connections.TryGetValue(key, out var connectedSet))
                connectedSet.Remove(value);
        }

        public IEnumerable<TPort> FindConnectedPorts(Guid portId)
        {
            var port = PortMap[portId];
            return Connections.TryGetValue(port, out var connected) ? connected : new HashSet<TPort>();
        }

        public IEnumerable<TNode> FindConnectedNode(Guid portId)
        {
            return FindConnectedPorts(portId).Select(port => NodeMap[port.NodeId]);
        }

        public virtual bool IsCompatible(TPort input, TPort output) => true;
        public virtual void OnConnected(TPort input, TPort output) {}
        public virtual void OnDisconnected(TPort input, TPort output) {}
    }
}