using System;

namespace GraphExt
{
    public readonly struct EdgeData : IEquatable<EdgeData>
    {
        public readonly PortId OutputPort;
        public readonly PortId InputPort;

        public EdgeData(PortId outputPort, PortId inputPort)
        {
            OutputPort = outputPort;
            InputPort = inputPort;
        }

        public bool Equals(EdgeData other)
        {
            return InputPort.Equals(other.InputPort) && OutputPort.Equals(other.OutputPort);
        }

        public override bool Equals(object obj)
        {
            return obj is EdgeData other && Equals(other);
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