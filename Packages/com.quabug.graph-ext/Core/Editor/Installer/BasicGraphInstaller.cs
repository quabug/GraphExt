using System;
using System.Collections.Generic;
using System.Linq;
using OneShot;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    [Serializable]
    public class BasicGraphInstaller<TNode> : IInstaller where TNode : INode<GraphRuntime<TNode>>
    {
        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IGraphViewFactory GraphViewFactory = new DefaultGraphViewFactory();

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public INodeViewFactory NodeViewFactory = new DefaultNodeViewFactory();

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IPortViewFactory PortViewFactory = new DefaultPortViewFactory();

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IEdgeViewFactory EdgeViewFactory = new DefaultEdgeViewFactory();

        [SerializedType(typeof(IViewPresenter), Nullable = false, InstantializableType = true, RenamePatter = @"\w*\.||")]
        public string[] Presenters;

        public void Install(Container container)
        {
            container.RegisterInstance(GraphViewFactory);
            container.RegisterInstance(NodeViewFactory);
            container.RegisterInstance(PortViewFactory);
            container.RegisterInstance(EdgeViewFactory);

            container.RegisterGraphRuntimeInstance(new GraphRuntime<TNode>());
            container.RegisterGraphView();

            container.RegisterBiDictionaryInstance(new BiDictionary<NodeId, Node>());
            container.RegisterBiDictionaryInstance(new BiDictionary<PortId, Port>());
            container.RegisterBiDictionaryInstance(new BiDictionary<EdgeId, Edge>());
            container.RegisterDictionaryInstance(new Dictionary<PortId, PortData>());

            container.RegisterTypeNameArraySingleton<IViewPresenter>(Presenters);

            Func<IReadOnlyDictionary<NodeId, TNode>, IReadOnlyDictionary<NodeId, Vector2>, ConvertToNodeData>
                toNodeData = NodeDataConvertor.ToNodeData;
            container.RegisterSingleton(() => container.Call<ConvertToNodeData>(toNodeData));

            Func<IReadOnlyDictionary<NodeId, TNode>, FindPortData> findPortData = PortDataConvertor.FindPorts;
            container.RegisterSingleton(() => container.Call<FindPortData>(findPortData));

            Func<GraphRuntime<TNode>, IReadOnlyDictionary<PortId, PortData>, IsEdgeCompatibleFunc>
                isCompatible = EdgeFunctions.CreateIsCompatibleFunc;
            container.RegisterSingleton(() => container.Call<IsEdgeCompatibleFunc>(isCompatible));

            Func<IReadOnlyDictionary<Port, PortId>, IsEdgeCompatibleFunc, GraphView.FindCompatiblePorts>
                findCompatible = EdgeFunctions.CreateFindCompatiblePortsFunc;
            container.RegisterSingleton(() => container.Call<GraphView.FindCompatiblePorts>(findCompatible));

            Func<GraphRuntime<TNode>, EdgeConnectFunc> connect = EdgeFunctions.Connect;
            container.RegisterSingleton(() => container.Call<EdgeConnectFunc>(connect));

            Func<GraphRuntime<TNode>, EdgeDisconnectFunc> disconnect = EdgeFunctions.Disconnect;
            container.RegisterSingleton(() => container.Call<EdgeDisconnectFunc>(disconnect));

            container.RegisterSingleton<Func<IEnumerable<NodeId>>>(() =>
            {
                var graph = container.Resolve<GraphRuntime<TNode>>();
                return () => graph.Nodes.Select(n => n.Item1);
            });

            container.RegisterSingleton<Func<IEnumerable<EdgeId>>>(() =>
            {
                var graph = container.Resolve<GraphRuntime<TNode>>();
                return () => graph.Edges;
            });
        }
    }
}