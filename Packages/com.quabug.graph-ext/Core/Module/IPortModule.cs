using System;
using System.Collections.Generic;
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

        ISet<IPortModule> Connected { get; }
        void Connect([NotNull] IPortModule port);
        void Disconnect([NotNull] IPortModule port);

        bool IsCompatible([NotNull] IPortModule port);
    }
}