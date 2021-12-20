using System;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Memory
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

    public static class PortEnumExtensions
    {
        public static Direction Convert(this PortDirection direction) => direction switch
        {
            PortDirection.Input => Direction.Input,
            PortDirection.Output => Direction.Output,
            _ => throw new NotImplementedException()
        };

        public static Orientation Convert(this PortOrientation orientation) => orientation switch
        {
            PortOrientation.Horizontal => Orientation.Horizontal,
            PortOrientation.Vertical => Orientation.Vertical,
            _ => throw new NotImplementedException()
        };

        public static Port.Capacity Convert(this PortCapacity capacity) => capacity switch
        {
            PortCapacity.Single => Port.Capacity.Single,
            PortCapacity.Multi => Port.Capacity.Multi,
            _ => throw new NotImplementedException()
        };
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class NodePortAttribute : Attribute
    {
        public bool Hide = false;
        public bool HideLabel = false;
        public Type PortType = null;
        public PortDirection Direction = PortDirection.Invalid;
        public PortCapacity Capacity = PortCapacity.Invalid;
        public PortOrientation Orientation = PortOrientation.Horizontal;
        public string Name = null;
    }
}