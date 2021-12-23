using System.Collections.Generic;

namespace GraphExt.Prefab
{
    public interface IPortComponent
    {
        PortId Id { get; }
        IEnumerable<PortId> ConnectedPorts { get; }
        void OnConnected(in PortId port);
        void OnDisconnected(in PortId port);
        bool IsCompatible(in PortId port);
    }
}