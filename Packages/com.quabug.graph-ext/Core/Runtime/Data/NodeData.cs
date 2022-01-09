using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public readonly struct NodeData
    {
        [NotNull] public readonly IReadOnlyDictionary<string, PortData> Ports;
        [NotNull] public readonly IReadOnlyList<INodeProperty> Properties;
        public NodeData([NotNull] IEnumerable<INodeProperty> properties, [NotNull] IEnumerable<PortData> ports)
        {
            Properties = properties.ToArray();
            Ports = ports.ToDictionary(port => port.Name, port => port);
        }
    }
}