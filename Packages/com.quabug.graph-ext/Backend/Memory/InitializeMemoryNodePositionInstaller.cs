#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace GraphExt.Editor
{
    public class InitializeMemoryNodePositionInstaller : IGraphInstaller
    {
        public void Install(Container container, TypeContainers typeContainers)
        {
            container.Register<InitializeNodePosition>((resolveContainer, contractType) =>
            {
                var nodes = container.Resolve<Dictionary<NodeId, Vector2>>();
                return (in NodeId id, Vector2 position) => nodes[id] = position;
            }).AsSelf();
        }
    }
}

#endif