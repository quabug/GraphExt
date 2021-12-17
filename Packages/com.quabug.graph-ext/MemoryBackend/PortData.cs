using System;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Memory
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NodePortAttribute : Attribute
    {
        public bool Hide = false;
        public bool HideLabel = false;
        public Type PortType = null;
        public Direction Direction = Direction.Input;
        public Port.Capacity Capacity = Port.Capacity.Single;
        public Orientation Orientation = Orientation.Horizontal;
        public string Name = null;
    }

    public class PortData : IPortModule
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