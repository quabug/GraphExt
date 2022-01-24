// using System;
// using System.Collections.Generic;
// using System.Linq;
// using JetBrains.Annotations;
// using UnityEditor;
// using UnityEditor.Experimental.GraphView;
// using UnityEngine;
//
// namespace GraphExt.Editor
// {
//     public class PrefabGraphSetup<TNode, TComponent> : IDisposable
//         where TNode : INode<GraphRuntime<TNode>>
//         where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
//     {
//         [NotNull] public GameObjectNodes<TNode, TComponent> Graph { get; }
//
//         public BiDictionary<NodeId, Node> NodeViews { get; } = new BiDictionary<NodeId, Node>();
//         public BiDictionary<PortId, Port> PortViews { get; } = new BiDictionary<PortId, Port>();
//         public BiDictionary<EdgeId, Edge> EdgeViews { get; } = new BiDictionary<EdgeId, Edge>();
//
//         public Dictionary<PortId, PortData> Ports { get; } = new Dictionary<PortId, PortData>();
//         public NodePositions<TComponent> NodePositions { get; }
//
//         public GraphRuntime<TNode> GraphRuntime => Graph.Runtime;
//         public GraphView GraphView { get; }
//
//         public NodeViewPresenter NodeViewPresenter { get; }
//         public EdgeViewPresenter EdgeViewPresenter { get; }
//         public ElementMovedEventEmitter ElementMovedEventEmitter { get; }
//
//         public FocusActiveNodePresenter<TComponent> FocusActiveNodePresenter { get; }
//         public ActiveSelectedNodePresenter<TComponent> ActiveSelectedNodePresenter { get; }
//
//         public PrefabGraphSetup([NotNull] GraphConfig config, [NotNull] GameObjectNodes<TNode, TComponent> graph)
//         {
//             Graph = graph;
//             var isRuntimeCompatible = EdgeFunctions.IsCompatible(GraphRuntime, Ports);
//             GraphView = new GraphView(
//                 (in PortId input, in PortId output) => isRuntimeCompatible(input, output) &&
//                                                        graph.IsPortCompatible(input, output),
//                 PortViews.Reverse
//             );
//             GraphView.SetupGridBackground();
//             GraphView.SetupMiniMap();
//             GraphView.SetupDefaultManipulators();
//
//             NodePositions = new NodePositions<TComponent>(
//                 Graph.NodeObjectMap,
//                 node => node.Position,
//                 (node, position) => node.Position = position
//             );
//
//             NodeViewPresenter = new NodeViewPresenter(
//                 GraphView,
//                 config.GetViewFactory<INodeViewFactory>(),
//                 config.GetViewFactory<IPortViewFactory>(),
//                 () => GraphRuntime.Nodes.Select(t => t.Item1),
//                 NodeDataConvertor.ToNodeData(id => Graph.NodeObjectMap[id], id => Graph.SerializedObjects[id]),
//                 PortDataConvertor.FindPorts(GraphRuntime.NodeMap),
//                 NodeViews,
//                 PortViews,
//                 Ports
//             );
//
//             EdgeViewPresenter = new EdgeViewPresenter(
//                 GraphView,
//                 config.GetViewFactory<IEdgeViewFactory>(),
//                 () => GraphRuntime.Edges,
//                 EdgeViews,
//                 PortViews,
//                 EdgeFunctions.Connect(GraphRuntime),
//                 EdgeFunctions.Disconnect(GraphRuntime)
//             );
//
//             ElementMovedEventEmitter = new ElementMovedEventEmitter(GraphView);
//
//             FocusActiveNodePresenter = new FocusActiveNodePresenter<TComponent>(
//                 GraphView,
//                 node => NodeViews[Graph[node]],
//                 () => Selection.activeGameObject == null ? null : Selection.activeGameObject.GetComponent<TComponent>()
//             );
//
//             ActiveSelectedNodePresenter = new ActiveSelectedNodePresenter<TComponent>(
//                 NodeViews,
//                 Graph.NodeObjectMap,
//                 node =>
//                 {
//                     if (Selection.activeGameObject != node.gameObject) Selection.activeGameObject = node.gameObject;
//                 }
//             );
//         }
//
//         public void Tick()
//         {
//             NodeViewPresenter.Tick();
//             EdgeViewPresenter.Tick();
//             ActiveSelectedNodePresenter.Tick();
//         }
//
//         public void Dispose()
//         {
//             EdgeViewPresenter?.Dispose();
//             ElementMovedEventEmitter?.Dispose();
//             FocusActiveNodePresenter?.Dispose();
//         }
//     }
// }
