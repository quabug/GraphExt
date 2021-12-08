using System.Collections.Generic;
using UnityEngine;

namespace GraphExt
{
    public interface INodeModule
    {
        Vector2 Position { get; set; }
        IEnumerable<INodeProperty> Properties { get; }
    }
}