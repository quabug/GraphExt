using System;

namespace GraphExt
{
    public readonly struct EdgeData : IEquatable<EdgeData>
    {
        public readonly PortId InputPort;
        public readonly PortId OutputPort;

        public EdgeData(PortId inputPort, PortId outputPort)
        {
            InputPort = inputPort;
            OutputPort = outputPort;
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