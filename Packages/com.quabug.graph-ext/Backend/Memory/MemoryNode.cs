namespace GraphExt
{
    public interface IMemoryNode : INode<GraphRuntime<IMemoryNode>> {}

    public abstract class MemoryNode : IMemoryNode
    {
        public bool IsPortCompatible(GraphRuntime<IMemoryNode> graph, in PortId start, in PortId end) => true;
        public void OnConnected(GraphRuntime<IMemoryNode> graph, in PortId start, in PortId end) {}
        public void OnDisconnected(GraphRuntime<IMemoryNode> graph, in PortId start, in PortId end) {}
    }
}