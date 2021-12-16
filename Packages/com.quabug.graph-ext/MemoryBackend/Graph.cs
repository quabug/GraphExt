using System;
using System.Collections.Generic;

namespace GraphExt.Memory
{
    [Serializable]
    public class Graph : GraphModule<Node, Port>
    {
        public IReadOnlyCollection<Node> NodeList => NodeMap.Values;

        public Graph() {}
        public Graph(IEnumerable<Node> nodes) : base(nodes) {}

        public override bool IsCompatible(Port input, Port output)
        {
            return input.Inner.IsCompatible(this, output.Inner) && output.Inner.IsCompatible(this, input.Inner);
        }

        public override void OnConnected(Port input, Port output)
        {
            input.Inner.OnConnected?.Invoke(this, output.Inner);
            output.Inner.OnConnected?.Invoke(this, input.Inner);
        }

        public override void OnDisconnected(Port input, Port output)
        {
            input.Inner.OnDisconnected?.Invoke(this, output.Inner);
            output.Inner.OnDisconnected?.Invoke(this, input.Inner);
        }

        public Node CreateNode(IMemoryNode innerNode)
        {
            var node = new Node(innerNode);
            NodeMap.Add(node.Id, node);
            node.OnDeleted += () => NodeMap.Remove(node.Id);
            return node;
        }
    }
}