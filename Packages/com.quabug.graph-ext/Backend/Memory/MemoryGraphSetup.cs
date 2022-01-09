using System;
using GraphExt;
using GraphExt.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GraphView = UnityEditor.Experimental.GraphView.GraphView;

public class MemoryGraphSetup<TNode> : IGraphSetup, IDisposable where TNode : INode<GraphRuntime<TNode>>
{
    public GraphElements<NodeId, Node> NodeViews { get; } = new GraphElements<NodeId, Node>();
    public GraphElements<PortId, Port> PortViews { get; } = new GraphElements<PortId, Port>();
    public GraphElements<EdgeId, Edge> EdgeViews { get; } = new GraphElements<EdgeId, Edge>();

    public ViewModuleElements<PortId, PortData> Ports { get; } = new ViewModuleElements<PortId, PortData>();
    public ViewModuleElements<NodeId, NodeData> Nodes { get; } = new ViewModuleElements<NodeId, NodeData>();
    public ViewModuleElements<NodeId, Vector2> NodePositions { get; } = new ViewModuleElements<NodeId, Vector2>();

    public INodeViewFactory NodeViewFactory { get; } = new DefaultNodeViewFactory();
    public IEdgeViewFactory EdgeViewFactory { get; } = new DefaultEdgeViewFactory();
    public IPortViewFactory PortViewFactory { get; } = new DefaultPortViewFactory();

    public GraphRuntime<TNode> GraphRuntime { get; } = new GraphRuntime<TNode>();
    public GraphView GraphView { get; }

    public IEdgesViewModule EdgesViewModule { get; }
    public IEdgeConnectionViewModule EdgeConnectionViewModule { get; }
    public INodesViewModule NodesViewModule { get; }

    public NodeViewPresenter NodeViewPresenter { get; }
    public EdgeViewPresenter EdgeViewPresenter { get; }
    public MenuBuilder MenuBuilder { get; }

    public MemoryGraphSetup()
    {
        EdgesViewModule = new EdgesViewModule<TNode>(GraphRuntime);
        EdgeConnectionViewModule = new EdgeConnectionViewModule<TNode>(GraphRuntime, Ports);
        NodesViewModule = new MemoryNodesViewModule<TNode>(GraphRuntime, NodePositions, Nodes, Ports);

        GraphView = new GraphExt.Editor.GraphView(EdgeConnectionViewModule, PortViews);

        NodeViewPresenter = new NodeViewPresenter(GraphView, NodeViewFactory, PortViewFactory, NodesViewModule, NodeViews, PortViews);
        EdgeViewPresenter = new EdgeViewPresenter(GraphView, EdgeViewFactory, EdgeConnectionViewModule, EdgesViewModule, EdgeViews, PortViews);
        MenuBuilder = new MenuBuilder(GraphView, new IMenuEntry[]
        {
            new SelectionEntry(),
            new MemoryNodeMenuEntry<TNode>(GraphRuntime, NodePositions),
        });
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
}