#if UNITY_EDITOR

using System;
using OneShot;
using UnityEngine;

namespace GraphExt.Editor
{
    public class InitializeScriptableObjectNodePositionInstaller : IGraphInstaller
    {
        public void Install(Container container, TypeContainers typeContainers)
        {
            container.RegisterSingleton<InitializeNodePosition>(() =>
            {
                var nodes = container.Resolve<Func<NodeId, NodeScriptableObject>>();
                return (in NodeId id, Vector2 position) => nodes(id).Position = position;
            });
        }
    }
}

#endif