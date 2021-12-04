#if UNITY_EDITOR

using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt
{
    public interface INodePort
    {
        Orientation Orientation { get; }
        Direction Direction { get; }
        Port.Capacity Capacity { get; }
        Type PortType { get; }
    }

    public static partial class NodePortExtension
    {
        public static Port CreatePortView([NotNull] this INodePort port)
        {
            var p = Port.Create<Edge>(port.Orientation, port.Direction, port.Capacity, port.PortType);
            p.portName = "";
            return p;
        }
    }
}

#endif