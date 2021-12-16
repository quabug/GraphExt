using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public interface INodeModule
    {
        string UiFile { get; }
        Guid Id { get; }
        Vector2 Position { get; set; }
        [NotNull] IReadOnlyList<INodeProperty> Properties { get; }
    }
}