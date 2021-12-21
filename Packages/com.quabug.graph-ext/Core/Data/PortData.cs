using System;
using UnityEditor.Experimental.GraphView;

namespace GraphExt
{
    public readonly struct PortData
    {
        public PortId Id { get; }
        public NodeId NodeId => Id.NodeId;
        public Orientation Orientation { get; }
        public Direction Direction { get; }
        public Port.Capacity Capacity { get; }
        public Type PortType { get; }

        public PortData(in PortId id, Orientation orientation, Direction direction, Port.Capacity capacity, Type portType)
        {
            Id = id;
            Orientation = orientation;
            Direction = direction;
            Capacity = capacity;
            PortType = portType;
        }
    }
}