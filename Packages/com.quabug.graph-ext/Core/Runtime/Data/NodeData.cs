using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public readonly struct NodeData
    {
        [NotNull] public readonly IReadOnlyList<INodeProperty> Properties;
        public NodeData([NotNull] IEnumerable<INodeProperty> properties) => Properties = properties.ToArray();
    }
}