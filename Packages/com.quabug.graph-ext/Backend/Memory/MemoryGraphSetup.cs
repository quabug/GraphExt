using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryGraphSetup<TNode> : IDisposable where TNode : INode<GraphRuntime<TNode>>
    {
        public GraphElements<NodeId, Node> NodeViews { get; } = new GraphElements<NodeId, Node>();
        public GraphElements<PortId, Port> PortViews { get; } = new GraphElements<PortId, Port>();
        public GraphElements<EdgeId, Edge> EdgeViews { get; } = new GraphElements<EdgeId, Edge>();

        public ViewModuleElements<PortId, PortData> Ports { get; } = new ViewModuleElements<PortId, PortData>();
        public ViewModuleElements<NodeId, NodeData> Nodes { get; } = new ViewModuleElements<NodeId, NodeData>();
        public ViewModuleElements<NodeId, Vector2> NodePositions { get; } = new ViewModuleElements<NodeId, Vector2>();

        public DefaultNodeViewFactory NodeViewFactory { get; } = new DefaultNodeViewFactory();
        public DefaultEdgeViewFactory EdgeViewFactory { get; } = new DefaultEdgeViewFactory();
        public DefaultPortViewFactory PortViewFactory { get; } = new DefaultPortViewFactory();

        public GraphRuntime<TNode> GraphRuntime { get; } = new GraphRuntime<TNode>();
        public GraphView GraphView { get; private set; }

        public EdgesViewModule<TNode> EdgesViewModule { get; private set; }
        public EdgeConnectionViewModule<TNode> EdgeConnectionViewModule { get; private set; }
        public MemoryNodesViewModule<TNode> NodesViewModule { get; private set; }

        public NodeViewPresenter NodeViewPresenter { get; private set; }
        public EdgeViewPresenter EdgeViewPresenter { get; private set; }
        public SyncNodePositionPresenter SyncNodePositionPresenter { get; private set; }

        public MemoryGraphSetup()
        {
            Setup();
        }

        public MemoryGraphSetup(IReadOnlyGraphRuntime<TNode> graphRuntime, IReadOnlyDictionary<NodeId, Vector2> positions)
        {
            GraphRuntime = new GraphRuntime<TNode>(graphRuntime);
            NodePositions = new ViewModuleElements<NodeId, Vector2>(positions);
            Setup();
        }

        public void Tick()
        {
            NodeViewPresenter.Tick();
            EdgeViewPresenter.Tick();
        }

        public void Dispose()
        {
            EdgeViewPresenter?.Dispose();
        }

        private void Setup()
        {
            EdgesViewModule = new EdgesViewModule<TNode>(GraphRuntime);
            EdgeConnectionViewModule = new EdgeConnectionViewModule<TNode>(GraphRuntime, Ports);
            NodesViewModule = new MemoryNodesViewModule<TNode>(GraphRuntime, NodePositions, Nodes, Ports);

            GraphView = new GraphView(EdgeConnectionViewModule, PortViews);

            NodeViewPresenter = new NodeViewPresenter(GraphView, NodeViewFactory, PortViewFactory, NodesViewModule, NodeViews, PortViews);
            EdgeViewPresenter = new EdgeViewPresenter(GraphView, EdgeViewFactory, EdgeConnectionViewModule, EdgesViewModule, EdgeViews, PortViews);
            SyncNodePositionPresenter = new SyncNodePositionPresenter(GraphView, NodeViews, NodePositions);
        }
    }
}