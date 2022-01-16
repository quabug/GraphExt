using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphSetup<TNode, TNodeScriptableObject> : IDisposable
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        [NotNull] public GraphScriptableObject<TNode, TNodeScriptableObject> Graph { get; }

        public BiDictionary<NodeId, Node> NodeViews { get; } = new BiDictionary<NodeId, Node>();
        public BiDictionary<PortId, Port> PortViews { get; } = new BiDictionary<PortId, Port>();
        public BiDictionary<EdgeId, Edge> EdgeViews { get; } = new BiDictionary<EdgeId, Edge>();

        public Dictionary<PortId, PortData> Ports { get; } = new Dictionary<PortId, PortData>();
        public ScriptableObjectNodePositions<TNode, TNodeScriptableObject> NodePositions { get; private set; }

        public DefaultNodeViewFactory NodeViewFactory { get; } = new DefaultNodeViewFactory();
        public DefaultEdgeViewFactory EdgeViewFactory { get; } = new DefaultEdgeViewFactory();
        public DefaultPortViewFactory PortViewFactory { get; } = new DefaultPortViewFactory();

        public GraphRuntime<TNode> GraphRuntime => Graph.Runtime;
        public GraphView GraphView { get; private set; }

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
            GraphView = new GraphView(EdgeFunctions.IsCompatible(GraphRuntime, Ports), PortViews.Reverse);

            NodePositions = new ScriptableObjectNodePositions<TNode, TNodeScriptableObject>(Graph);

            NodeViewPresenter = new NodeViewPresenter(
                GraphView,
                NodeViewFactory,
                PortViewFactory,
                ScriptableObjectFunctions.ToNodeData<TNode, TNodeScriptableObject>(Graph.NodeObjectMap),
                () => GraphRuntime.Nodes.Select(t => t.Item1),
                NodeViews,
                PortViews,
                Ports
            );

            EdgeViewPresenter = new EdgeViewPresenter(
                GraphView,
                EdgeViewFactory,
                () => GraphRuntime.Edges,
                EdgeViews,
                PortViews,
                EdgeFunctions.Connect(GraphRuntime),
                EdgeFunctions.Disconnect(GraphRuntime)
            );

            SyncNodePositionPresenter = new SyncNodePositionPresenter(
                GraphView,
                NodeViews.Reverse,
                NodePositions
            );
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