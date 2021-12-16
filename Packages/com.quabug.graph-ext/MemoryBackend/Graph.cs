using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Memory
{
    [Serializable]
    public class Graph : IGraphModule
    {
        public IEnumerable<INodeModule> Nodes => _nodeList.Values;
        private readonly Dictionary<Guid, Node> _nodeList;
        private readonly Dictionary<Port, ISet<Port>> _connected = new Dictionary<Port, ISet<Port>>();
        public IReadOnlyCollection<Node> NodeList => _nodeList.Values;
        public IReadOnlyDictionary<Port, ISet<Port>> ConnectedPorts => _connected;

        private ISet<EdgeData> _edgeCache = new HashSet<EdgeData>();
        public IEnumerable<EdgeData> Edges => _edgeCache;

        public Graph()
        {
            _nodeList = new Dictionary<Guid, Node>();
        }

        public Graph(IEnumerable<Node> nodeList)
        {
            _nodeList = nodeList.ToDictionary(n => n.Id, n => n);
        }

        public bool IsCompatible(IPortModule input, IPortModule output)
        {
            return input.Direction != output.Direction &&
                   input.PortType == output.PortType &&
                   input is Port @in && output is Port @out &&
                   @in.Inner.IsCompatible(this, @out.Inner) && @out.Inner.IsCompatible(this, @in.Inner)
            ;
        }

        public void Connect(IPortModule input, IPortModule output)
        {
            if (input is Port @in && output is Port @out)
            {
                AddConnection(@in, @out);
                AddConnection(@out, @in);
                @in.Inner.OnConnected?.Invoke(this, @out.Inner);
                @out.Inner.OnConnected?.Invoke(this, @in.Inner);
            }
        }

        public void Disconnect(IPortModule input, IPortModule output)
        {
            if (input is Port @in && output is Port @out)
            {
                RemoveConnection(@in, @out);
                RemoveConnection(@out, @in);
                @in.Inner.OnDisconnected?.Invoke(this, @out.Inner);
                @out.Inner.OnDisconnected?.Invoke(this, @in.Inner);
            }
        }

        private void AddConnection(Port key, Port value)
        {
            if (!_connected.TryGetValue(key, out var connectedSet))
            {
                connectedSet = new HashSet<Port>();
                _connected.Add(key, connectedSet);
            }

            if (!connectedSet.Contains(value)) connectedSet.Add(value);

            if (key.Direction == Direction.Output)
                _edgeCache.Add(new EdgeData(key.Id, value.Id));
        }

        private void RemoveConnection(Port key, Port value)
        {
            if (_connected.TryGetValue(key, out var connectedSet))
                connectedSet.Remove(value);

            if (key.Direction == Direction.Output)
                _edgeCache.Remove(new EdgeData(key.Id, value.Id));
        }

        internal Node CreateNode(IMemoryNode innerNode)
        {
            var node = new Node(innerNode);
            _nodeList.Add(node.Id, node);
            node.OnDeleted += () => _nodeList.Remove(node.Id);
            return node;
        }

        public Node FindNodeByPort(Port port)
        {
            return _nodeList[port.Id.NodeId];
        }

        public ISet<Port> FindConnectedPorts(in PortId id)
        {
            var node = _nodeList[id.NodeId];
            var port = node.Ports[id.PortIndex];
            return _connected.TryGetValue(port, out var connected) ? connected : new HashSet<Port>();
        }

        public ISet<Node> FindConnectedNode(in PortId id)
        {
            return new HashSet<Node>(FindConnectedPorts(id).Select(FindNodeByPort));
        }
    }
}