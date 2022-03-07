using System;
using System.Collections.Generic;
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
            container.RegisterInstance(GraphViewFactory).AsInterfaces();
            container.RegisterInstance(NodeViewFactory).AsInterfaces();
            container.RegisterInstance(PortViewFactory).AsInterfaces();
            container.RegisterInstance(EdgeViewFactory).AsInterfaces();

            RegisterGraphView(container, typeContainers);

            container.RegisterBiDictionaryInstance(new BiDictionary<NodeId, Node>());
            container.RegisterBiDictionaryInstance(new BiDictionary<PortId, Port>());
            container.RegisterBiDictionaryInstance(new BiDictionary<EdgeId, Edge>());
            container.RegisterDictionaryInstance(new Dictionary<PortId, PortData>());

            RegisterNodeViewPresenter(container, typeContainers);
            RegisterEdgeViewPresenter(container, typeContainers);
            container.Register<ElementMovedEventEmitter>().Singleton().AsSelf().As<IWindowSystem>();
        }

        void RegisterGraphView(Container container, TypeContainers typeContainers)
        {
            var graphContainer = typeContainers.CreateTypeContainer(
                container,
                typeof(IGraphViewFactory),
                typeof(GraphView),
                typeof(UnityEditor.Experimental.GraphView.GraphView)
            );

            Func<GraphRuntime<TNode>, IReadOnlyDictionary<PortId, PortData>, IsEdgeCompatibleFunc>
                isCompatible = EdgeFunctions.CreateIsCompatibleFunc;
            graphContainer.Register((resolveContainer, contractType) => graphContainer.Call<IsEdgeCompatibleFunc>(isCompatible)).AsSelf();

            Func<IReadOnlyDictionary<Port, PortId>, IsEdgeCompatibleFunc, GraphView.FindCompatiblePorts>
                findCompatible = EdgeFunctions.CreateFindCompatiblePortsFunc;
            graphContainer.Register((resolveContainer, contractType) => graphContainer.Call<GraphView.FindCompatiblePorts>(findCompatible)).AsSelf();

            graphContainer.Register((resolveContainer, contractType) =>
            {
                Func<GraphView.FindCompatiblePorts, GraphView> create = graphContainer.Resolve<IGraphViewFactory>().Create;
                return graphContainer.Call<GraphView>(create);
            }).Singleton().AsSelf().As<UnityEditor.Experimental.GraphView.GraphView>();

            container.Register((resolverContainer, contractType) => graphContainer.Resolve<UnityEditor.Experimental.GraphView.GraphView>()).Singleton().AsSelf();
        }

        void RegisterNodeViewPresenter(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.CreateSystemContainer(container, typeof(NodeViewPresenter));

            presenterContainer.Register<ConvertToNodeData>((resolveContainer, contractType) => NodeDataConvertor.ToNodeData(
                presenterContainer.Resolve<IReadOnlyDictionary<NodeId, TNode>>(),
                presenterContainer.Resolve<IReadOnlyDictionary<NodeId, Vector2>>()
            )).AsSelf();

            presenterContainer.Register<FindPortData>((resolveContainer, contractType) => PortDataConvertor.FindPorts(
                presenterContainer.Resolve<IReadOnlyDictionary<NodeId, TNode>>()
            )).AsSelf();

            // TODO: RX?
            presenterContainer.Register((resolveContainer, contractType) =>
            {
                var graphRuntime = presenterContainer.Resolve<GraphRuntime<TNode>>();
                var added = new NodeViewPresenter.NodeAddedEvent();
                graphRuntime.OnNodeAdded += (in NodeId id, TNode _) => added.Event?.Invoke(id);
                return added;
            }).AsSelf();

            presenterContainer.Register((resolveContainer, contractType) =>
            {
                var graphRuntime = presenterContainer.Resolve<GraphRuntime<TNode>>();
                var deleted = new NodeViewPresenter.NodeDeletedEvent();
                graphRuntime.OnNodeWillDelete += (in NodeId id, TNode _) => deleted.Event?.Invoke(id);
                return deleted;
            }).AsSelf();
        }

        void RegisterEdgeViewPresenter(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.CreateSystemContainer(container, typeof(EdgeViewInitializer), typeof(EdgeViewObserver), typeof(EdgeRuntimeObserver<TNode>));
            presenterContainer.Register<IEnumerable<EdgeId>>((resolveContainer, contractType) => container.Resolve<GraphRuntime<TNode>>().Edges).AsSelf();
            presenterContainer.Register((resolveContainer, contractType) => EdgeFunctions.Connect(presenterContainer.Resolve<GraphRuntime<TNode>>())).AsSelf();
            presenterContainer.Register((resolveContainer, contractType) => EdgeFunctions.Disconnect(presenterContainer.Resolve<GraphRuntime<TNode>>())).AsSelf();
        }
    }
}