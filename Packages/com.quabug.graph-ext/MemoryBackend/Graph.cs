using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphExt.Memory
{
    [Serializable]
    public class Graph : IGraphModule
    {
        public IEnumerable<INodeModule> Nodes => _nodeList;
        private readonly List<Node> _nodeList;
        private readonly Dictionary<Port, ISet<Port>> _connected =
            new Dictionary<Port, ISet<Port>>();

        public IReadOnlyList<Node> NodeList => _nodeList;
        public IReadOnlyDictionary<Port, ISet<Port>> ConnectedPorts => _connected;

        public Graph()
        {
            _nodeList = new List<Node>();
        }

        public Graph(List<Node> nodeList)
        {
            _nodeList = nodeList;
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
        }

        private void RemoveConnection(Port key, Port value)
        {
            if (_connected.TryGetValue(key, out var connectedSet))
            {
                connectedSet.Remove(value);
            }
        }

        internal Node CreateNode(IMemoryNode innerNode)
        {
            var node = new Node(innerNode);
            _nodeList.Add(node);
            node.OnDeleted += () => _nodeList.Remove(node);
            return node;
        }

        public IMemoryNode FindNodeByPort(IMemoryPort port)
        {
            if (port == null) return null;
            return null;
        }

        public ISet<IMemoryPort> FindConnectedPorts(IMemoryPort port)
        {
            return new HashSet<IMemoryPort>();
            // return _connected.TryGetValue(port, out var connected) ? connected : new HashSet<IMemoryPort>();
        }

        public ISet<IMemoryNode> FindConnectedNode(IMemoryPort port)
        {
            return new HashSet<IMemoryNode>(FindConnectedPorts(port).Select(FindNodeByPort));
        }
    }
}