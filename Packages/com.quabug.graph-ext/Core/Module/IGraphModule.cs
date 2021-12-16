using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphExt
{
    public interface IGraphModule
    {
        [NotNull] IEnumerable<INodeModule> Nodes { get; }
        bool IsCompatible([NotNull] IPortModule input, [NotNull] IPortModule output);
        void Connect([NotNull] IPortModule input, [NotNull] IPortModule output);
        void Disconnect([NotNull] IPortModule input, [NotNull] IPortModule output);
    }
}