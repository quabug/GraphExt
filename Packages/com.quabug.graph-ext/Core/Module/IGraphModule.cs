using System.Collections.Generic;

namespace GraphExt
{
    public interface IGraphModule
    {
        string Name { get; }
        IEnumerable<IGraphNode> Nodes { get; }
    }

}