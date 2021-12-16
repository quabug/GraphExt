using System;

namespace GraphExt
{
    public readonly struct EdgeId : IEquatable<EdgeId>
    {
        public readonly Guid OutputPort;
        public readonly Guid InputPort;

        public EdgeId(Guid outputPort, Guid inputPort)
        {
            OutputPort = outputPort;
            InputPort = inputPort;
        }

        public bool Equals(EdgeId other)
        {
            return InputPort.Equals(other.InputPort) && OutputPort.Equals(other.OutputPort);
        }

        public override bool Equals(object obj)
        {
            return obj is EdgeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (InputPort.GetHashCode() * 397) ^ OutputPort.GetHashCode();
            }
        }
    }
}