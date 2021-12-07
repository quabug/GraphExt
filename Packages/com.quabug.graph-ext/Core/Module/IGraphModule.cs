using System.Collections.Generic;

namespace GraphExt
{
    public interface IGraphModule
    {
        IEnumerable<IGraphNode> Nodes { get; }
    }

}