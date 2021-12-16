using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Memory
{
    public interface IMemoryPort
    {
        int Id { get; set; }
        bool IsCompatible(Graph graph, IMemoryPort port);
        Action<Graph, IMemoryPort> OnConnected { get; }
        Action<Graph, IMemoryPort> OnDisconnected { get; }
    }

    public class MemoryPort : IMemoryPort
    {
        public int Id { get; set; }
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

        public Orientation Orientation => Orientation.Horizontal;
        public Direction Direction { get; }
        public UnityEditor.Experimental.GraphView.Port.Capacity Capacity { get; }
        public Type PortType { get; }

        public Guid NodeId { get; }
        public int Index { get; }

        public Port(Guid nodeId, int index, [NotNull] IMemoryPort inner, Type portType, Direction direction, UnityEditor.Experimental.GraphView.Port.Capacity capacity)
        {
            NodeId = nodeId;
            inner.Id = Index;
            Index = index;
            Inner = inner;
            Direction = direction;
            Capacity = capacity;
            PortType = portType;
        }
    }
}