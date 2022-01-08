using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt.Editor
{
    public readonly struct NodeElement
    {
        public readonly NodeData Data;
        public readonly IReadOnlyList<PortData> Ports;

        public NodeElement(in NodeData data, IEnumerable<PortData> ports)
        {
            Data = data;
            Ports = ports.ToArray();
        }
    }

    public interface INodesViewModule
    {
    }

    public abstract class NodesViewModule : INodesViewModule
    {
        public NodesViewModule() {}

        [NotNull] protected abstract IReadOnlyDictionary<string/*portName*/, PortData> FindNodePorts(in NodeId nodeId);
        protected abstract NodeData ToNodeData(in NodeId nodeId);
    }
}