using System;
using JetBrains.Annotations;

namespace GraphExt
{
    public readonly struct PortId : IEquatable<PortId>
    {
        public readonly NodeId NodeId;
        public readonly string Name;

        public PortId(in NodeId nodeId, [NotNull] string portName)
        {
            NodeId = nodeId;
            Name = portName;
        }

        public static bool operator ==(in PortId lhs, in PortId rhs)
        {
            return lhs.NodeId == rhs.NodeId && lhs.Name == rhs.Name;
        }

        public static bool operator !=(in PortId lhs, in PortId rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(PortId other)
        {
            return NodeId.Equals(other.NodeId) && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is PortId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (NodeId.GetHashCode() * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}