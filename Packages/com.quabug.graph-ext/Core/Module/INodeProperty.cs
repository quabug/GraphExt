using System.Collections.Generic;

namespace GraphExt
{
    public interface INodeProperty
    {
        IEnumerable<IPortModule> Ports { get; }
    }
}