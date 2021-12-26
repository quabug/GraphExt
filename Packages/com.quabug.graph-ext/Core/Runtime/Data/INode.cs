namespace GraphExt
{
    public interface INode<in TGraph>
    {
        bool IsPortCompatible(TGraph graph, in PortId start, in PortId end);
        void OnConnected(TGraph graph, in PortId start, in PortId end);
        void OnDisconnected(TGraph graph, in PortId start, in PortId end);
    }
}