using System;

namespace GraphExt.Memory
{
    public interface IMemoryNode
    {
        NodeId Id { get; }
        bool IsPortCompatible(Graph graph, in PortId start, in PortId end);
        void OnConnected(Graph graph, in PortId start, in PortId end);
        void OnDisconnected(Graph graph, in PortId start, in PortId end);
    }

    public abstract class MemoryNode : IMemoryNode
    {
        public NodeId Id { get; set; /* for deserialization */ } = Guid.NewGuid();
        public virtual bool IsPortCompatible(Graph graph, in PortId start, in PortId end) => true;
        public virtual void OnConnected(Graph graph, in PortId start, in PortId end) {}
        public virtual void OnDisconnected(Graph graph, in PortId start, in PortId end) {}
    }
}