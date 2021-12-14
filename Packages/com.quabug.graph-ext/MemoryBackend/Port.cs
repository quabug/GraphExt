using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;

namespace GraphExt.Memory
{
    public interface IMemoryPort
    {
        IMemoryNode Node { get; }
        ISet<IMemoryPort> ConnectedPorts { get; }
        bool IsCompatible(IMemoryPort port);
        void OnConnected(IMemoryPort port);
        void OnDisconnected(IMemoryPort port);
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

        public ISet<IPortModule> Connected { get; } = new HashSet<IPortModule>();

        public Port([NotNull] IMemoryPort inner, Direction direction, UnityEditor.Experimental.GraphView.Port.Capacity capacity, Type portType)
        {
            Inner = inner;
            Direction = direction;
            Capacity = capacity;
            PortType = portType;
        }

        public void Connect(IPortModule port)
        {
            Assert.IsFalse(Connected.Contains(port), "already connected");
            Assert.IsTrue(IsCompatible(port), "not compatible");
            Assert.IsTrue(port is Port, "connect to `Port` only");
            Connected.Add(port);
            Inner.OnConnected(((Port)port).Inner);
        }

        public void Disconnect(IPortModule port)
        {
            Assert.IsTrue(Connected.Contains(port), "already disconnected");
            Assert.IsTrue(port is Port, "disconnect from `Port` only");
            Connected.Remove(port);
            Inner.OnDisconnected(((Port)port).Inner);
        }

        public bool IsCompatible(IPortModule port)
        {
            return !Connected.Contains(port) &&
                   port.Direction != Direction &&
                   port.PortType == PortType &&
                   port is Port p && Inner.IsCompatible(p.Inner)
           ;
        }
    }
}