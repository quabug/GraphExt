using System;

namespace GraphExt.Memory
{
    public interface IMemoryNode
    {
        bool IsPortCompatible(MemoryGraphBackend memoryGraphBackend, in PortId start, in PortId end);
        void OnConnected(MemoryGraphBackend memoryGraphBackend, in PortId start, in PortId end);
        void OnDisconnected(MemoryGraphBackend memoryGraphBackend, in PortId start, in PortId end);
    }

    public abstract class MemoryNode : IMemoryNode
    {
        public virtual bool IsPortCompatible(MemoryGraphBackend memoryGraphBackend, in PortId start, in PortId end) => true;
        public virtual void OnConnected(MemoryGraphBackend memoryGraphBackend, in PortId start, in PortId end) {}
        public virtual void OnDisconnected(MemoryGraphBackend memoryGraphBackend, in PortId start, in PortId end) {}
    }
}