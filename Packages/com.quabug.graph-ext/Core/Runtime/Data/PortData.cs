using System;

namespace GraphExt
{
    public enum PortDirection
    {
        Input, Output, Invalid
    }

    public enum PortOrientation
    {
        Horizontal, Vertical, Invalid
    }

    public enum PortCapacity
    {
        Single, Multi, Invalid
    }

    public readonly struct PortData
    {
        public PortId Id { get; }
        public NodeId NodeId => Id.NodeId;
        public PortOrientation Orientation { get; }
        public PortDirection Direction { get; }
        public PortCapacity Capacity { get; }
        public Type PortType { get; }

        public PortData(in PortId id, PortOrientation orientation, PortDirection direction, PortCapacity capacity, Type portType)
        {
            Id = id;
            Orientation = orientation;
            Direction = direction;
            Capacity = capacity;
            PortType = portType;
        }
    }

#if UNITY_EDITOR
    public static class PortEnumExtensions
    {
        public static UnityEditor.Experimental.GraphView.Direction ToEditor(this PortDirection direction) => direction switch
        {
            PortDirection.Input => UnityEditor.Experimental.GraphView.Direction.Input,
            PortDirection.Output => UnityEditor.Experimental.GraphView.Direction.Output,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        public static UnityEditor.Experimental.GraphView.Orientation ToEditor(this PortOrientation orientation) => orientation switch
        {
            PortOrientation.Horizontal => UnityEditor.Experimental.GraphView.Orientation.Horizontal,
            PortOrientation.Vertical => UnityEditor.Experimental.GraphView.Orientation.Vertical,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };

        public static UnityEditor.Experimental.GraphView.Port.Capacity ToEditor(this PortCapacity capacity) => capacity switch
        {
            PortCapacity.Single => UnityEditor.Experimental.GraphView.Port.Capacity.Single,
            PortCapacity.Multi => UnityEditor.Experimental.GraphView.Port.Capacity.Multi,
            _ => throw new ArgumentOutOfRangeException(nameof(capacity), capacity, null)
        };
    }
#endif
}