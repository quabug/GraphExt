#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    [Serializable]
    public class PrefabGraphBackendInstaller<TNode, TNodeComponent> : IGraphInstaller
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeComponent: MonoBehaviour, INodeComponent<TNode, TNodeComponent>
    {
        [SerializeField] private BasicGraphInstaller<TNode> _basicGraphInstaller;
        [SerializeField] private SerializableGraphBackendInstaller<TNode, TNodeComponent> _serializableGraphBackendInstaller;

        public void Install(Container container, TypeContainers typeContainers)
        {
            _basicGraphInstaller.Install(container, typeContainers);
            _serializableGraphBackendInstaller.Install(container, typeContainers);
            container.Register<InitializeNodePosition>((resolveContainer, contractType) =>
            {
                var nodes = container.Resolve<Func<NodeId, INodeComponent>>();
                return (in NodeId id, Vector2 position) => nodes(id).Position = position;
            }).AsSelf();
            OverrideEdgeCompatibleFunc();

            typeContainers.GetTypeContainer<SyncSelectionGraphElementPresenter>().Register<PrefabNodeSelectionConvertor<NodeId, Node, TNodeComponent>>().Singleton().AsSelf().AsInterfaces();

            void OverrideEdgeCompatibleFunc()
            {
                var graphContainer = typeContainers.GetTypeContainer(typeof(IGraphViewFactory));
                graphContainer.Register<IsEdgeCompatibleFunc>((resolveContainer, contractType) =>
                {
                    var graph = container.Resolve<GameObjectNodes<TNode, TNodeComponent>>();
                    var ports = container.Resolve<IReadOnlyDictionary<PortId, PortData>>();
                    var isRuntimePortCompatible = EdgeFunctions.CreateIsCompatibleFunc(graph.Runtime, ports);
                    return (in PortId input, in PortId output) =>
                        isRuntimePortCompatible(input, output) && graph.IsPortCompatible(input, output)
                    ;
                }).AsSelf();
            }
        }
    }
}

#endif
