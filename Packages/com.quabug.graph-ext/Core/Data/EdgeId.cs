using System;

namespace GraphExt
{
    public readonly struct EdgeId : IEquatable<EdgeId>
    {
        public readonly PortId First;
        public readonly PortId Second;

        public EdgeId(PortId first, PortId second)
        {
            First = first;
            Second = second;
        }

        public bool Equals(EdgeId other)
        {
            return (Second.Equals(other.Second) && First.Equals(other.First)) ||
                   (Second.Equals(other.First) && First.Equals(other.Second))
            ;
        }

        public override bool Equals(object obj)
        {
            return obj is EdgeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Second.GetHashCode() ^ First.GetHashCode()) * 397;
            }
        }

        public static bool operator ==(EdgeId lhs, EdgeId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(EdgeId lhs, EdgeId rhs)
        {
            return !(lhs == rhs);
        }
    }
}