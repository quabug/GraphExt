using System;
using System.Collections.Generic;
using OneShot;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    [Serializable]
    public class BasicGraphInstaller<TNode> : IGraphInstaller where TNode : INode<GraphRuntime<TNode>>
    {
        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IGraphViewFactory GraphViewFactory = new DefaultGraphViewFactory();

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public INodeViewFactory NodeViewFactory = new DefaultNodeViewFactory();

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IPortViewFactory PortViewFactory = new DefaultPortViewFactory();

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IEdgeViewFactory EdgeViewFactory = new DefaultEdgeViewFactory();

        public void Install(Container container, TypeContainers typeContainers)
        {
            container.RegisterInstance(GraphViewFactory);
            container.RegisterInstance(NodeViewFactory);
            container.RegisterInstance(PortViewFactory);
            container.RegisterInstance(EdgeViewFactory);

            RegisterGraphView(container);

            container.RegisterBiDictionaryInstance(new BiDictionary<NodeId, Node>());
            container.RegisterBiDictionaryInstance(new BiDictionary<PortId, Port>());
            container.RegisterBiDictionaryInstance(new BiDictionary<EdgeId, Edge>());
            container.RegisterDictionaryInstance(new Dictionary<PortId, PortData>());

            RegisterNodeViewPresenter(container, typeContainers);
            RegisterEdgeViewPresenter(container, typeContainers);
            container.RegisterSingleton<ElementMovedEventEmitter>();
            container.RegisterSingleton<IWindowSystem>(container.Resolve<ElementMovedEventEmitter>);
        }

        void RegisterGraphView(Container container)
        {
            Func<GraphRuntime<TNode>, IReadOnlyDictionary<PortId, PortData>, IsEdgeCompatibleFunc>
                isCompatible = EdgeFunctions.CreateIsCompatibleFunc;
            container.RegisterSingleton(() => container.Call<IsEdgeCompatibleFunc>(isCompatible));

            Func<IReadOnlyDictionary<Port, PortId>, IsEdgeCompatibleFunc, GraphView.FindCompatiblePorts>
                findCompatible = EdgeFunctions.CreateFindCompatiblePortsFunc;
            container.RegisterSingleton(() => container.Call<GraphView.FindCompatiblePorts>(findCompatible));

            container.RegisterSingleton(() =>
            {
                Func<GraphView.FindCompatiblePorts, GraphView> create = container.Resolve<IGraphViewFactory>().Create;
                return container.Call<GraphView>(create);
            });
            container.Register<UnityEditor.Experimental.GraphView.GraphView>(container.Resolve<GraphView>);
        }

        void RegisterNodeViewPresenter(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.CreateSystemContainer(container, typeof(NodeViewPresenter));

            presenterContainer.RegisterSingleton(() => NodeDataConvertor.ToNodeData(
                presenterContainer.Resolve<IReadOnlyDictionary<NodeId, TNode>>(),
                presenterContainer.Resolve<IReadOnlyDictionary<NodeId, Vector2>>()
            ));

            presenterContainer.RegisterSingleton(() => PortDataConvertor.FindPorts(
                presenterContainer.Resolve<IReadOnlyDictionary<NodeId, TNode>>()
            ));

            // TODO: RX?
            presenterContainer.RegisterSingleton(() =>
            {
                var graphRuntime = presenterContainer.Resolve<GraphRuntime<TNode>>();
                var added = new NodeViewPresenter.NodeAddedEvent();
                graphRuntime.OnNodeAdded += (in NodeId id, TNode _) => added.Event?.Invoke(id);
                return added;
            });

            presenterContainer.RegisterSingleton(() =>
            {
                var graphRuntime = presenterContainer.Resolve<GraphRuntime<TNode>>();
                var deleted = new NodeViewPresenter.NodeDeletedEvent();
                graphRuntime.OnNodeWillDelete += (in NodeId id, TNode _) => deleted.Event?.Invoke(id);
                return deleted;
            });
        }

        void RegisterEdgeViewPresenter(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.CreateSystemContainer(container, typeof(EdgeViewInitializer), typeof(EdgeViewObserver));
            presenterContainer.RegisterSingleton(() => EdgeFunctions.Connect(presenterContainer.Resolve<GraphRuntime<TNode>>()));
            presenterContainer.RegisterSingleton(() => EdgeFunctions.Disconnect(presenterContainer.Resolve<GraphRuntime<TNode>>()));
        }
    }
}