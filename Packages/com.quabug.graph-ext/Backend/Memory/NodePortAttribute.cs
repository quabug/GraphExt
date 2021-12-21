using System;

namespace GraphExt.Memory
{
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