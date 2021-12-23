namespace GraphExt.Memory
{
    public interface IMemoryNode
    {
        bool IsPortCompatible(MemoryGraphBackend graph, in PortId start, in PortId end);
        void OnConnected(MemoryGraphBackend graph, in PortId start, in PortId end);
        void OnDisconnected(MemoryGraphBackend graph, in PortId start, in PortId end);
    }

    public abstract class MemoryNode : IMemoryNode
    {
        public virtual bool IsPortCompatible(MemoryGraphBackend graph, in PortId start, in PortId end) => true;
        public virtual void OnConnected(MemoryGraphBackend graph, in PortId start, in PortId end) {}
        public virtual void OnDisconnected(MemoryGraphBackend graph, in PortId start, in PortId end) {}
    }
}