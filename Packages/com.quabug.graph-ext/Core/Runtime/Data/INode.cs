namespace GraphExt
{
    public interface INode<in TGraph>
    {
        bool IsPortCompatible(TGraph graph, in PortId input, in PortId output);
        void OnConnected(TGraph graph, in PortId input, in PortId output);
        void OnDisconnected(TGraph graph, in PortId input, in PortId output);
    }
}