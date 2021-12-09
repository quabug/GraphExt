using System.Collections.Generic;

namespace GraphExt.Memory
{
    public class Graph : IGraphModule
    {
        public IList<Node> NodeList = new List<Node>();
        public IEnumerable<INodeModule> Nodes => NodeList;

        internal Node CreateNode(IMemoryNode innerNode)
        {
            var node = new Node(innerNode);
            NodeList.Add(node);
            node.OnDeleted += () => NodeList.Remove(node);
            return node;
        }
    }
}