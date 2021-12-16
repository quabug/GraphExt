using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Memory
{
    public interface IMemoryPort
    {
        PortId Id { get; set; }
        bool IsCompatible(Graph graph, IMemoryPort port);
        Action<Graph, IMemoryPort> OnConnected { get; }
        Action<Graph, IMemoryPort> OnDisconnected { get; }
    }

    public class MemoryPort : IMemoryPort
    {
        public PortId Id { get; set;  }
        public Action<Graph, IMemoryPort> OnConnected { get; }
        public Action<Graph, IMemoryPort> OnDisconnected { get; }
        public virtual bool IsCompatible(Graph graph, IMemoryPort port) => true;
    }

    [Flags]
    public enum NodePortDirection
    {
        Input = 1 << 0, Output = 1 << 1
    }

    [AttributeUsage(AttributeTargets.Field)]
    [BaseTypeRequired(typeof(IMemoryPort))]
    public class NodePortAttribute : Attribute
    {
        public bool AllowMultipleConnections = false;
        public NodePortDirection Direction = NodePortDirection.Input | NodePortDirection.Output;
        public Type PortType { get; }

        public NodePortAttribute(Type portType)
        {
            PortType = portType;
        }
    }

    public class Port : IPortModule
    {
        public IMemoryPort Inner { get; }

        public PortId Id { get; }
        public Orientation Orientation => Orientation.Horizontal;
        public Direction Direction { get; }
        public UnityEditor.Experimental.GraphView.Port.Capacity Capacity { get; }
        public Type PortType { get; }

        public Port(in PortId id, [NotNull] IMemoryPort inner, Type portType, Direction direction, UnityEditor.Experimental.GraphView.Port.Capacity capacity)
        {
            inner.Id = id;

            Id = id;
            Inner = inner;
            Direction = direction;
            Capacity = capacity;
            PortType = portType;
        }
    }
}