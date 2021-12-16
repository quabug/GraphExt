using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public interface INodeModule : IDisposable
    {
        Guid Id { get; }
        Vector2 Position { get; set; }
        IEnumerable<INodeProperty> Properties { get; }
        [CanBeNull] IPortModule FindPort(in PortId port);
    }
}