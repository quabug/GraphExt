using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphSetup<TNode, TNodeScriptableObject>
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        [NotNull] public GraphScriptableObject<TNode, TNodeScriptableObject> Graph { get; }

        public GraphElements<NodeId, Node> NodeViews { get; } = new GraphElements<NodeId, Node>();
        public GraphElements<PortId, Port> PortViews { get; } = new GraphElements<PortId, Port>();
        public GraphElements<EdgeId, Edge> EdgeViews { get; } = new GraphElements<EdgeId, Edge>();

        public ViewModuleElements<PortId, PortData> Ports { get; } = new ViewModuleElements<PortId, PortData>();
        public ViewModuleElements<NodeId, NodeData> Nodes { get; } = new ViewModuleElements<NodeId, NodeData>();
        public ViewModuleElements<NodeId, Vector2> NodePositions { get; } = new ViewModuleElements<NodeId, Vector2>();

        public DefaultNodeViewFactory NodeViewFactory { get; } = new DefaultNodeViewFactory();
        public DefaultEdgeViewFactory EdgeViewFactory { get; } = new DefaultEdgeViewFactory();
        public DefaultPortViewFactory PortViewFactory { get; } = new DefaultPortViewFactory();

        public GraphRuntime<TNode> GraphRuntime => Graph.Runtime;
        public GraphView GraphView { get; private set; }

        public EdgesViewModule<TNode> EdgesViewModule { get; private set; }
        public EdgeConnectionViewModule<TNode> EdgeConnectionViewModule { get; private set; }

        public NodeViewPresenter NodeViewPresenter { get; private set; }
        public EdgeViewPresenter EdgeViewPresenter { get; private set; }
        public SyncNodePositionPresenter SyncNodePositionPresenter { get; private set; }

        public ScriptableObjectGraphSetup([NotNull] GraphScriptableObject<TNode, TNodeScriptableObject> graph)
        {
            Graph = graph;
        }
    }
}