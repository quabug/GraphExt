using System;
using System.Collections.Generic;

namespace GraphExt.Memory
{
    [Serializable]
    public class Graph : Graph<Node>
    {
        public Graph() {}
        public Graph(IEnumerable<Node> nodes, IEnumerable<EdgeId> edges) : base(nodes, edges) {}

        public override bool IsCompatible(PortData input, PortData output)
        {
            return input.Direction != output.Direction &&
                   input.PortType == output.PortType &&
                   GetInnerNode(input).IsPortCompatible(this, output.Id, input.Id) &&
                   GetInnerNode(output).IsPortCompatible(this, output.Id, input.Id)
            ;
        }

        private IMemoryNode GetInnerNode(PortData port) => _NodeMap[port.NodeId].Inner;

        public override void OnConnected(PortData input, PortData output)
        {
            GetInnerNode(input).OnConnected(this, output.Id, input.Id);
            GetInnerNode(output).OnConnected(this, output.Id, input.Id);
        }

        public override void OnDisconnected(PortData input, PortData output)
        {
            GetInnerNode(input).OnDisconnected(this, output.Id, input.Id);
            GetInnerNode(output).OnDisconnected(this, output.Id, input.Id);
        }

        public Node CreateNode(IMemoryNode innerNode)
        {
            var node = new Node(innerNode);
            _NodeMap.Add(node.Id, node);
            foreach (var port in node.Ports) _PortMap.Add(port.Id, port);
            return node;
        }
    }
}