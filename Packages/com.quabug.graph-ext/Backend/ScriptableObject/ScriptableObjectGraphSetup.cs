using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphSetup<TNode, TNodeScriptableObject> : IDisposable
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        [NotNull] public GraphScriptableObject<TNode, TNodeScriptableObject> Graph { get; }

        public GraphElements<NodeId, Node> NodeViews { get; } = new GraphElements<NodeId, Node>();
        public GraphElements<PortId, Port> PortViews { get; } = new GraphElements<PortId, Port>();
        public GraphElements<EdgeId, Edge> EdgeViews { get; } = new GraphElements<EdgeId, Edge>();

        public ViewModuleElements<PortId, PortData> Ports { get; } = new ViewModuleElements<PortId, PortData>();
        public ViewModuleElements<NodeId, NodeData> Nodes { get; } = new ViewModuleElements<NodeId, NodeData>();
        public ScriptableObjectNodePositions<TNode, TNodeScriptableObject> NodePositions { get; private set; }

        public DefaultNodeViewFactory NodeViewFactory { get; } = new DefaultNodeViewFactory();
        public DefaultEdgeViewFactory EdgeViewFactory { get; } = new DefaultEdgeViewFactory();
        public DefaultPortViewFactory PortViewFactory { get; } = new DefaultPortViewFactory();

        public GraphRuntime<TNode> GraphRuntime => Graph.Runtime;
        public GraphView GraphView { get; private set; }

        public EdgesViewModule<TNode> EdgesViewModule { get; private set; }
        public EdgeConnectionViewModule<TNode> EdgeConnectionViewModule { get; private set; }
        public NodesViewModule<TNode> NodesViewModule { get; private set; }

        public NodeViewPresenter NodeViewPresenter { get; private set; }
        public EdgeViewPresenter EdgeViewPresenter { get; private set; }
        public SyncNodePositionPresenter SyncNodePositionPresenter { get; private set; }

        public ScriptableObjectGraphSetup([NotNull] GraphScriptableObject<TNode, TNodeScriptableObject> graph)
        {
            Graph = graph;
            graph.Initialize();
            Setup();
        }

        private void Setup()
        {
            NodePositions = new ScriptableObjectNodePositions<TNode, TNodeScriptableObject>(Graph);

            EdgesViewModule = new EdgesViewModule<TNode>(GraphRuntime);
            EdgeConnectionViewModule = new EdgeConnectionViewModule<TNode>(GraphRuntime, Ports);
            NodesViewModule = new NodesViewModule<TNode>(GraphRuntime, NodePositions, Nodes, Ports);

            GraphView = new GraphView(EdgeConnectionViewModule, PortViews);

            NodeViewPresenter = new NodeViewPresenter(GraphView, NodeViewFactory, PortViewFactory, NodesViewModule, NodeViews, PortViews);
            EdgeViewPresenter = new EdgeViewPresenter(GraphView, EdgeViewFactory, EdgeConnectionViewModule, EdgesViewModule, EdgeViews, PortViews);
            SyncNodePositionPresenter = new SyncNodePositionPresenter(GraphView, NodeViews, NodePositions);
        }

        public void Tick()
        {
            NodeViewPresenter.Tick();
            EdgeViewPresenter.Tick();
        }

        public void Dispose()
        {
            EdgeViewPresenter?.Dispose();
            SyncNodePositionPresenter?.Dispose();
        }
    }
}