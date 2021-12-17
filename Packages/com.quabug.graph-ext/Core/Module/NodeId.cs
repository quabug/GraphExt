using System;

namespace GraphExt
{
    public readonly struct NodeId : IEquatable<NodeId>
    {
        public readonly Guid Id;
        public NodeId(Guid id) => Id = id;
        public static explicit operator Guid(NodeId nodeId) => nodeId.Id;
        public static implicit operator NodeId(Guid id) => new NodeId(id);

        public bool Equals(NodeId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is NodeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(in NodeId lhs, in NodeId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(in NodeId lhs, in NodeId rhs)
        {
            return !(lhs == rhs);
        }
    }
}