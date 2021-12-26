using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public sealed class GraphRuntime<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        private readonly BiDictionary<NodeId, TNode> _nodeMap = new BiDictionary<NodeId, TNode>();
        private readonly HashSet<PortId> _ports = new HashSet<PortId>();
        private readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        // TODO: IReadOnlySet / ImmutableSet
        public ISet<PortId> Ports => _ports;
        public ISet<EdgeId> Edges => _edges;
        public IReadOnlyDictionary<NodeId, TNode> NodeMap => _nodeMap.Forward;

        public TNode this[in NodeId id] => _nodeMap[id];

        public void AddNode(in NodeId id, TNode node, IEnumerable<string> portNames)
        {
            _nodeMap[id] = node;
            foreach (var portName in portNames) _ports.Add(new PortId(id, portName));
        }

        public void DeleteNode(in NodeId nodeId)
        {
            if (_nodeMap.ContainsKey(nodeId))
            {
                _nodeMap.Remove(nodeId);
                RemoveNodePorts(nodeId);
            }
        }

        public TNode GetNodeByPort(in PortId port) => this[port.NodeId];

        public bool IsCompatible(in PortId input, in PortId output)
        {
            return GetNodeByPort(input).IsPortCompatible(this, output, input) &&
                   GetNodeByPort(output).IsPortCompatible(this, output, input)
            ;
        }

        public void Connect(in PortId input, in PortId output)
        {
            Edges.Add(new EdgeId(input, output));
            GetNodeByPort(input).OnConnected(this, output, input);
            GetNodeByPort(output).OnConnected(this, output, input);
        }

        public void Disconnect(in PortId input, in PortId output)
        {
            Edges.Remove(new EdgeId(input, output));
            GetNodeByPort(input).OnDisconnected(this, output, input);
            GetNodeByPort(output).OnDisconnected(this, output, input);
        }


        [NotNull] public IEnumerable<PortId> FindConnectedPorts(TNode node, string port)
        {
            return FindConnectedPorts(new PortId(_nodeMap.GetKey(node), port));
        }

        [NotNull] public IEnumerable<PortId> FindConnectedPorts(PortId portId)
        {
            return _edges.SelectMany(edge => edge.GetConnectedPort(portId));
        }

        [NotNull] public IEnumerable<NodeId> FindConnectedNodes(in PortId portId)
        {
            return FindConnectedPorts(portId).Select(port => port.NodeId);
        }

        [NotNull] public IEnumerable<PortId> FindNodePorts(NodeId nodeId)
        {
            return _ports.Where(port => port.NodeId == nodeId);
        }

        private void RemoveNodePorts(in NodeId nodeId)
        {
            var removedPorts = new HashSet<PortId>();
            foreach (var port in _ports)
            {
                if (port.NodeId == nodeId) removedPorts.Add(port);
            }

            foreach (var port in removedPorts)
            {
                _ports.Remove(port);
                RemovePortEdges(port);
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
                Edges.Remove(edge);
            }
        }
    }
}