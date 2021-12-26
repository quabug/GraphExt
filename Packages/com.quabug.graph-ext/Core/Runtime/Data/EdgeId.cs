using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphExt
{
    public readonly struct EdgeId : IEquatable<EdgeId>
    {
        public readonly PortId Input;
        public readonly PortId Output;

        public EdgeId(in PortId input, in PortId output)
        {
            Input = input;
            Output = output;
        }

        public bool Contains(in PortId portId) => Input == portId || Output == portId;

        public IEnumerable<PortId> GetConnectedPort(in PortId portId)
        {
            if (Input == portId) return Output.Yield();
            if (Output == portId) return Input.Yield();
            return Enumerable.Empty<PortId>();
        }

        public bool Equals(EdgeId other)
        {
            return (Output.Equals(other.Output) && Input.Equals(other.Input)) ||
                   (Output.Equals(other.Input) && Input.Equals(other.Output))
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
                return (Output.GetHashCode() ^ Input.GetHashCode()) * 397;
            }
        }

        public static bool operator ==(in EdgeId lhs, in EdgeId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(in EdgeId lhs, in EdgeId rhs)
        {
            return !(lhs == rhs);
        }
    }
}