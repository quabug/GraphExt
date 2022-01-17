using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryGraphSetup<TNode> : IDisposable where TNode : INode<GraphRuntime<TNode>>
    {
        public BiDictionary<NodeId, Node> NodeViews { get; } = new BiDictionary<NodeId, Node>();
        public BiDictionary<PortId, Port> PortViews { get; } = new BiDictionary<PortId, Port>();
        public BiDictionary<EdgeId, Edge> EdgeViews { get; } = new BiDictionary<EdgeId, Edge>();

        public Dictionary<PortId, PortData> Ports { get; } = new Dictionary<PortId, PortData>();
        public Dictionary<NodeId, Vector2> NodePositions { get; } = new Dictionary<NodeId, Vector2>();

        public DefaultNodeViewFactory NodeViewFactory { get; } = new DefaultNodeViewFactory();
        public DefaultEdgeViewFactory EdgeViewFactory { get; } = new DefaultEdgeViewFactory();
        public DefaultPortViewFactory PortViewFactory { get; } = new DefaultPortViewFactory();

        public GraphRuntime<TNode> GraphRuntime { get; } = new GraphRuntime<TNode>();
        public GraphView GraphView { get; private set; }

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
            NodePositions = positions.ToDictionary(t => t.Key, t => t.Value);
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
            GraphView = new GraphView(EdgeFunctions.IsCompatible(GraphRuntime, Ports), PortViews.Reverse);

            NodeViewPresenter = new NodeViewPresenter(
                GraphView,
                NodeViewFactory,
                PortViewFactory,
                () => GraphRuntime.Nodes.Select(t => t.Item1),
                NodeDataConvertor.ToNodeData(GraphRuntime.NodeMap, NodePositions),
                PortDataConvertor.FindPorts(GraphRuntime.NodeMap),
                NodeViews, PortViews, Ports);

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
    }
}