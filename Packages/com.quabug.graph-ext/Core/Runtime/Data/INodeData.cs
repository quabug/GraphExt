using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphExt
{
    public interface INodeData
    {
        NodeId Id { get; }
        [NotNull] IReadOnlyList<INodeProperty> Properties { get; }
        [NotNull] IReadOnlyList<PortData> Ports { get; }
    }
}