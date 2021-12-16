using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt
{
    public interface IGraphModule
    {
        [NotNull] IEnumerable<INodeModule> Nodes { get; }
        [NotNull] IEnumerable<EdgeData> Edges { get; }
        bool IsCompatible([NotNull] IPortModule input, [NotNull] IPortModule output);
        void Connect([NotNull] IPortModule input, [NotNull] IPortModule output);
        void Disconnect([NotNull] IPortModule input, [NotNull] IPortModule output);
    }

    public class GraphModule<TNode, TPort> : IGraphModule where TNode : INodeModule where TPort : IPortModule
    {
        protected readonly Dictionary<Guid, TNode> NodeMap;
        protected readonly Dictionary<TPort, ISet<TPort>> Connections = new Dictionary<TPort, ISet<TPort>>();

        public IEnumerable<INodeModule> Nodes => NodeMap.Values.Cast<INodeModule>();

        private readonly ISet<EdgeData> _edgeCache = new HashSet<EdgeData>();
        public IEnumerable<EdgeData> Edges => _edgeCache;

        public GraphModule()
        {
            NodeMap = new Dictionary<Guid, TNode>();
        }

        public GraphModule(IEnumerable<TNode> nodeList)
        {
            NodeMap = nodeList.ToDictionary(n => n.Id, n => n);
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
                _edgeCache.Add(new EdgeData(outputPort: output.Id, inputPort: input.Id));
                OnConnected(@in, @out);
            }
        }

        public void Disconnect(IPortModule input, IPortModule output)
        {
            if (input is TPort @in && output is TPort @out)
            {
                RemoveConnection(@in, @out);
                RemoveConnection(@out, @in);
                _edgeCache.Remove(new EdgeData(output.Id, input.Id));
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

        public TPort FindPort(in PortId port)
        {
            return (TPort)NodeMap[port.NodeId].FindPort(port);
        }

        public TNode FindNodeByPort(in PortId port)
        {
            return NodeMap[port.NodeId];
        }

        public ISet<TPort> FindConnectedPorts(in PortId id)
        {
            var node = NodeMap[id.NodeId];
            var port = (TPort)node.FindPort(id);
            return Connections.TryGetValue(port, out var connected) ? connected : new HashSet<TPort>();
        }

        public ISet<TNode> FindConnectedNode(in PortId id)
        {
            return new HashSet<TNode>(FindConnectedPorts(id).Select(port => FindNodeByPort(port.Id)));
        }

        public virtual bool IsCompatible(TPort input, TPort output) => true;
        public virtual void OnConnected(TPort input, TPort output) {}
        public virtual void OnDisconnected(TPort input, TPort output) {}
    }
}