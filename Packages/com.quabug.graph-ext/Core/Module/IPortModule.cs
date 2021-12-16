using System;
using UnityEditor.Experimental.GraphView;

namespace GraphExt
{
    public interface IPortModule
    {
        Orientation Orientation { get; }
        Direction Direction { get; }
        Port.Capacity Capacity { get; }
        Type PortType { get; }
    }
}