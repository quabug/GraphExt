using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
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
        public NodePositions<TNodeScriptableObject> NodePositions { get; }

        public GraphRuntime<TNode> GraphRuntime => Graph.Runtime;
        public GraphView GraphView { get; }

        public NodeViewPresenter NodeViewPresenter { get; }
        public EdgeViewPresenter EdgeViewPresenter { get; }
        public SyncNodePositionPresenter SyncNodePositionPresenter { get; }

        public FocusActiveNodePresenter<TNodeScriptableObject> FocusActiveNodePresenter { get; }
        public ActiveSelectedNodePresenter<TNodeScriptableObject> ActiveSelectedNodePresenter { get; }

        public ScriptableObjectGraphSetup([NotNull] GraphConfig config, [NotNull] GraphScriptableObject<TNode, TNodeScriptableObject> graph)
        {
            Graph = graph;
            graph.Initialize();

            GraphView = new GraphView(EdgeFunctions.IsCompatible(GraphRuntime, Ports), PortViews.Reverse);
            GraphView.SetupGridBackground();
            GraphView.SetupMiniMap();
            GraphView.SetupDefaultManipulators();

            NodePositions = new NodePositions<TNodeScriptableObject>(
                Graph.NodeObjectMap,
                node => node.Position,
                (node, position) => node.Position = position
            );

            NodeViewPresenter = new NodeViewPresenter(
                GraphView,
                config.GetViewFactory<INodeViewFactory>(),
                config.GetViewFactory<IPortViewFactory>(),
                () => GraphRuntime.Nodes.Select(t => t.Item1),
                NodeDataConvertor.ToNodeData(id => Graph.NodeObjectMap[id]),
                PortDataConvertor.FindPorts(GraphRuntime.NodeMap),
                NodeViews,
                PortViews,
                Ports
            );

            EdgeViewPresenter = new EdgeViewPresenter(
                GraphView,
                config.GetViewFactory<IEdgeViewFactory>(),
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

            FocusActiveNodePresenter = new FocusActiveNodePresenter<TNodeScriptableObject>(
                GraphView,
                node => NodeViews[Graph[node]],
                () => Selection.activeObject as TNodeScriptableObject
            );

            ActiveSelectedNodePresenter = new ActiveSelectedNodePresenter<TNodeScriptableObject>(
                NodeViews,
                Graph.NodeObjectMap,
                node =>
                {
                    if (Selection.activeObject != node) Selection.activeObject = node;
                }
            );
        }

        public void Tick()
        {
            NodeViewPresenter.Tick();
            EdgeViewPresenter.Tick();
            ActiveSelectedNodePresenter.Tick();
        }

        public void Dispose()
        {
            EdgeViewPresenter?.Dispose();
            SyncNodePositionPresenter?.Dispose();
            FocusActiveNodePresenter?.Dispose();
        }
    }
}