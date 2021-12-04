using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphExt
{
    public interface IGraphNode : IDisposable
    {
        int Id { get; }
        Vector2 Position { get; set; }
        IEnumerable<INodeProperty> Properties { get; }
    }
}