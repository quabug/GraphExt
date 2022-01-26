using System;

namespace GraphExt
{
    public readonly struct PortData : IEquatable<PortData>
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

        public PortData AddClass(params string[] additionalClasses)
        {
            var classes = new string[AdditionalClasses.Length + additionalClasses.Length];
            Array.Copy(AdditionalClasses, classes, AdditionalClasses.Length);
            Array.Copy(additionalClasses, 0, classes, AdditionalClasses.Length, additionalClasses.Length);
            return new PortData(Name, Orientation, Direction, Capacity, PortType, classes);
        }

        public bool Equals(PortData other)
        {
            return Name == other.Name && Orientation == other.Orientation && Direction == other.Direction && Capacity == other.Capacity && Equals(PortType, other.PortType) && Equals(AdditionalClasses, other.AdditionalClasses);
        }

        public override bool Equals(object obj)
        {
            return obj is PortData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Orientation;
                hashCode = (hashCode * 397) ^ (int)Direction;
                hashCode = (hashCode * 397) ^ Capacity;
                hashCode = (hashCode * 397) ^ (PortType != null ? PortType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AdditionalClasses != null ? AdditionalClasses.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}