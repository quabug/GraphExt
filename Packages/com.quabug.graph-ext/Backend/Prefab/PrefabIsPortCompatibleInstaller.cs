#if UNITY_EDITOR

using System.Collections.Generic;
using OneShot;
using UnityEngine;

namespace GraphExt.Editor
{
    public class PrefabIsPortCompatibleInstaller<TNode, TComponent> : IGraphInstaller
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        public void Install(Container container, TypeContainers typeContainers)
        {
            var graphContainer = typeContainers.GetTypeContainer(typeof(IGraphViewFactory));
            graphContainer.RegisterSingleton<IsEdgeCompatibleFunc>(() =>
            {
                var graph = container.Resolve<GameObjectNodes<TNode, TComponent>>();
                var ports = container.Resolve<IReadOnlyDictionary<PortId, PortData>>();
                var isRuntimePortCompatible = EdgeFunctions.CreateIsCompatibleFunc(graph.Runtime, ports);
                return (in PortId input, in PortId output) =>
                    isRuntimePortCompatible(input, output) && graph.IsPortCompatible(input, output)
                ;
            });
        }
    }
}

#endif