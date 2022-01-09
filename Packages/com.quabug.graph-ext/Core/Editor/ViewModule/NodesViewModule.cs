using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphExt.Editor
{
    public interface INodesViewModule
    {
        [NotNull] IReadOnlyDictionary<NodeId, NodeData> GetNodes();
    }
}