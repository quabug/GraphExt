using System;

namespace GraphExt
{
    public readonly struct PortData
    {
        public readonly string Name;
        public readonly PortOrientation Orientation;
        public readonly PortDirection Direction;
        public readonly int Capacity;
        public readonly Type PortType;
        public readonly string[] AdditionalClasses;

        public PortData(string name, PortOrientation orientation, PortDirection direction, int capacity, Type portType, string[] additionalClasses)
        {
            Name = name;
            Orientation = orientation;
            Direction = direction;
            Capacity = capacity;
            PortType = portType;
            AdditionalClasses = additionalClasses;
        }

        public PortData AddClass(string @class)
        {
            var classes = new string[AdditionalClasses.Length + 1];
            Array.Copy(AdditionalClasses, classes, AdditionalClasses.Length);
            classes[AdditionalClasses.Length] = @class;
            return new PortData(Name, Orientation, Direction, Capacity, PortType, classes);
        }
    }
}