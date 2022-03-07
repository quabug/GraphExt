#if UNITY_EDITOR

using System;
using UnityEngine;

namespace GraphExt.Editor
{
    public class InitializeScriptableObjectNodePositionInstaller : IGraphInstaller
    {
        public void Install(Container container, TypeContainers typeContainers)
        {
            container.Register<InitializeNodePosition>((resolveContainer, contractType) =>
            {
                var nodes = container.Resolve<Func<NodeId, NodeScriptableObject>>();
                return (in NodeId id, Vector2 position) => nodes(id).Position = position;
            }).AsSelf();
        }
    }
}

#endif