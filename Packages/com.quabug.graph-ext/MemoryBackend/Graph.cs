using System.Collections.Generic;

namespace GraphExt.Memory
{
    public class Graph : IGraphModule
    {
        public IList<Node> NodeList = new List<Node>();
        public IEnumerable<IGraphNode> Nodes => NodeList;
    }
}