using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphExt
{
    public readonly struct NodeData
    {
        [NotNull] public readonly IReadOnlyList<INodeProperty> Properties;
        public NodeData([NotNull] IReadOnlyList<INodeProperty> properties) => Properties = properties;
    }
}