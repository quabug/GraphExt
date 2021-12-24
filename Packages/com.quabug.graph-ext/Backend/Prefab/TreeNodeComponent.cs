using System.Collections.Generic;

namespace GraphExt.Prefab
{
    public class TreeNodeComponent : NodeComponent
    {
        public override IEnumerable<EdgeId> Connections { get; }
    }
}