using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphExt.Editor
{
    public interface INodesViewModule
    {
        [NotNull] IEnumerable<(NodeId id, NodeData data)> GetNodes();
    }
}