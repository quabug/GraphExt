using System.Collections.Generic;

namespace GraphExt.Prefab
{
    public interface INodePropertyContainer
    {
        IEnumerable<INodeProperty> Properties { get; }
    }
}