using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public interface IGraphViewModule
    {
        [NotNull] IEnumerable<(PortId id, PortData data)> PortMap { get; }
        [NotNull] IEnumerable<(NodeId id, NodeData data)> NodeMap { get; }
        [NotNull] IEnumerable<EdgeId> Edges { get; }

        void DeleteNode(in NodeId nodeId);
        bool IsCompatible(in PortId input, in PortId output);
        void Connect(in PortId input, in PortId output);
        void Disconnect(in PortId input, in PortId output);
    }

    public interface INodeProperty {}

    public readonly struct NodeData
    {
        [NotNull] public readonly IReadOnlyList<INodeProperty> Properties;
        public NodeData([NotNull] IReadOnlyList<INodeProperty> properties) => Properties = properties;
    }

    public readonly struct PortData
    {
        public readonly string Name;
        public readonly Orientation Orientation;
        public readonly Direction Direction;
        public readonly Port.Capacity Capacity;
        public readonly Type PortType;

        public PortData(string name, Orientation orientation, Direction direction, Port.Capacity capacity, Type portType)
        {
            Name = name;
            Orientation = orientation;
            Direction = direction;
            Capacity = capacity;
            PortType = portType;
        }
    }
}