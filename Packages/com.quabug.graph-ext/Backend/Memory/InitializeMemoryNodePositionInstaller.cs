#if UNITY_EDITOR

using System.Collections.Generic;
using OneShot;
using UnityEngine;

namespace GraphExt.Editor
{
    public class InitializeMemoryNodePositionInstaller : IGraphInstaller
    {
        public void Install(Container container, TypeContainers typeContainers)
        {
            container.RegisterSingleton<InitializeNodePosition>(() =>
            {
                var nodes = container.Resolve<Dictionary<NodeId, Vector2>>();
                return (in NodeId id, Vector2 position) => nodes[id] = position;
            });
        }
    }
}

#endif