using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class PrefabGraphSetup<TNode, TComponent> : IDisposable
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        [NotNull] public GameObjectNodes<TNode, TComponent> Graph { get; }

        public BiDictionary<NodeId, Node> NodeViews { get; } = new BiDictionary<NodeId, Node>();
        public BiDictionary<PortId, Port> PortViews { get; } = new BiDictionary<PortId, Port>();
        public BiDictionary<EdgeId, Edge> EdgeViews { get; } = new BiDictionary<EdgeId, Edge>();

        public Dictionary<PortId, PortData> Ports { get; } = new Dictionary<PortId, PortData>();
        public NodePositions<TComponent> NodePositions { get; }

        public DefaultNodeViewFactory NodeViewFactory { get; } = new DefaultNodeViewFactory();
        public DefaultEdgeViewFactory EdgeViewFactory { get; } = new DefaultEdgeViewFactory();
        public DefaultPortViewFactory PortViewFactory { get; } = new DefaultPortViewFactory();

        public GraphRuntime<TNode> GraphRuntime => Graph.Runtime;
        public GraphView GraphView { get; }

        public NodeViewPresenter NodeViewPresenter { get; }
        public EdgeViewPresenter EdgeViewPresenter { get; }
        public ElementMovedEventEmitter ElementMovedEventEmitter { get; }

        public PrefabGraphSetup([NotNull] GameObjectNodes<TNode, TComponent> graph)
        {
            Graph = graph;
            var isRuntimeCompatible = EdgeFunctions.IsCompatible(GraphRuntime, Ports);
            GraphView = new GraphView(
                (in PortId input, in PortId output) => isRuntimeCompatible(input, output) &&
                                                       graph.IsPortCompatible(input, output),
                PortViews.Reverse
            );

            NodePositions = new NodePositions<TComponent>(
                Graph.NodeObjectMap,
                node => node.Position,
                (node, position) => node.Position = position
            );

            NodeViewPresenter = new NodeViewPresenter(
                GraphView,
                NodeViewFactory,
                PortViewFactory,
                NodeDataConvertor.ToNodeData(Graph.NodeObjectMap, node => NodePortUtility.FindPorts(node.Node)),
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

            ElementMovedEventEmitter = new ElementMovedEventEmitter(GraphView);
        }

        public void Tick()
        {
            NodeViewPresenter.Tick();
            EdgeViewPresenter.Tick();
        }

        public void Dispose()
        {
            EdgeViewPresenter?.Dispose();
            ElementMovedEventEmitter?.Dispose();
        }
    }
}
