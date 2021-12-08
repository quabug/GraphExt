using System.Collections.Generic;

namespace GraphExt
{
    public interface IGraphModule
    {
        IEnumerable<INodeModule> Nodes { get; }
    }
}