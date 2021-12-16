using System;
using UnityEditor.Experimental.GraphView;

namespace GraphExt
{
    public interface IPortModule
    {
        Guid Id { get; }
        Guid NodeId { get; }
        Orientation Orientation { get; }
        Direction Direction { get; }
        Port.Capacity Capacity { get; }
        Type PortType { get; }
    }
}