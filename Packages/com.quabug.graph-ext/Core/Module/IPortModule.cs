using System;
using UnityEditor.Experimental.GraphView;

namespace GraphExt
{
    public readonly struct PortId : IEquatable<PortId>
    {
        public readonly Guid NodeId;
        public readonly int PortIndex;

        public PortId(Guid nodeId, int portIndex)
        {
            NodeId = nodeId;
            PortIndex = portIndex;
        }

        public bool Equals(PortId other)
        {
            return NodeId.Equals(other.NodeId) && PortIndex == other.PortIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is PortId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (NodeId.GetHashCode() * 397) ^ PortIndex;
            }
        }
    }

    public interface IPortModule
    {
        PortId Id { get; }
        Orientation Orientation { get; }
        Direction Direction { get; }
        Port.Capacity Capacity { get; }
        Type PortType { get; }
    }
}