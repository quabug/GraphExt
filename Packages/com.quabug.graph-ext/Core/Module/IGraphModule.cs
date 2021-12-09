using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphExt
{
    public interface IGraphModule
    {
        [NotNull] IEnumerable<INodeModule> Nodes { get; }
    }
}