using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt
{
    public interface IPortModule
    {
        Orientation Orientation { get; }
        Direction Direction { get; }
        Port.Capacity Capacity { get; }
        Type PortType { get; }
    }

    public static partial class NodePortExtension
    {
        public static Port CreatePortView([NotNull] this IPortModule portModule)
        {
            var p = Port.Create<Edge>(portModule.Orientation, portModule.Direction, portModule.Capacity, portModule.PortType);
            p.portName = "";
            return p;
        }
    }
}