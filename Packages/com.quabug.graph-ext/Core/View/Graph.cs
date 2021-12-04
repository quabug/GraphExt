using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinaryEgo.Editor.UI;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class Graph : GraphView
    {
        public new class UxmlFactory : UxmlFactory<Graph, UxmlTraits> {}

        private IGraphModule _graph;
        // private IDictionary<int, Node> _nodes = new Dictionary<int, Node>();

        public Graph()
        {
            Insert(0, new GridBackground { name = "grid" });

            var miniMap = new MiniMap();
            Add(miniMap);
            // NOTE: not working... have to set `graphView` on `CreateGUI` of `BehaviorTreeEditor`
            miniMap.graphView = this;
            miniMap.windowed = true;
            miniMap.name = "minimap";

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public void Reset([CanBeNull] IGraphModule graph)
        {
            graphViewChanged -= OnGraphChanged;

            // _graph?.Dispose();
            _graph = graph;
            // _nodes.Clear();
            DeleteElements(graphElements.ToList());

            if (_graph != null)
            {
                // foreach (var node in _graph.Nodes) CreateNode(node);
                // foreach (var node in _nodes.Values.Cast<IConnectableVariantViewContainer>()) CreateEdges(node);
            }

            graphViewChanged += OnGraphChanged;

            GraphViewChange OnGraphChanged(GraphViewChange @event)
            {
                if (@event.elementsToRemove != null)
                {
                    foreach (var edge in @event.elementsToRemove.OfType<Edge>()) OnEdgeDeleted(edge);
                    foreach (var node in @event.elementsToRemove.OfType<INodeView>().ToArray())
                    {
                        // _nodes.Remove(node.Id);
                        node.Dispose();
                        edges.ForEach(edge =>
                        {
                            if (edge.input.node == node || edge.output.node == node)
                                @event.elementsToRemove.Add(edge);
                        });
                    }
                }

                if (@event.edgesToCreate != null)
                {
                    foreach (var edge in @event.edgesToCreate) OnEdgeCreated(edge, ref @event);
                }

                if (@event.movedElements != null)
                {
                    foreach (var node in @event.movedElements.OfType<INodeView>()) node.SyncPosition();
                }

                return @event;
            }

            void OnEdgeCreated(Edge edge, ref GraphViewChange @event)
            {
                edge.showInMiniMap = true;
                if (edge.input.node is NodeView inputNode && edge.output.node is NodeView outputNode)
                {
                    // outputNode.ConnectTo(inputNode);
                }
                else
                {
                    // var view = FindConnectableVariantView(edge);
                    // if (view != null)
                    // {
                    //     view.Connect(edge);
                    //
                    //     // disconnect edges on the same ports of ConnectableVariantView
                    //     edges.ForEach(e =>
                    //     {
                    //         if (e != edge && view.IsConnected(e))
                    //         {
                    //             e.input.Disconnect(e);
                    //             e.output.Disconnect(e);
                    //             RemoveElement(e);
                    //         }
                    //     });
                    //     // NOTE: not working since remove events are not handled in following process of `Port`
                    //     // var removedEdges = edges.ToList().Where(e => e != edge && nodePropertyView.IsConnected(e));
                    //     // @event.elementsToRemove ??= new List<GraphElement>();
                    //     // @event.elementsToRemove.AddRange(removedEdges);
                    // }
                }
            }

            void OnEdgeDeleted(Edge edge)
            {
                if (edge.input.node is NodeView inputNode && edge.output.node is NodeView outputNode)
                {
                    // inputNode.DisconnectFrom(outputNode);
                }
                else
                {
                    // var view = FindConnectableVariantView(edge);
                    // if (view != null) view.Disconnect(edge);
                }
            }
            //
            // ConnectableVariantView FindConnectableVariantView(Edge edge)
            // {
            //     return FindConnectableVariantViewByPort(edge.input) ?? FindConnectableVariantViewByPort(edge.output);
            // }
            //
            // ConnectableVariantView FindConnectableVariantViewByPort(Port port)
            // {
            //     if (port.node is IConnectableVariantViewContainer container)
            //         return container.FindByPort(port);
            //     return null;
            // }
        }

        // public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        // {
        //     var compatiblePorts = new List<Port>();
        //     ports.ForEach(port =>
        //     {
        //         var isCompatibleBehaviorPorts =
        //             port.orientation == Orientation.Vertical &&
        //             port.direction != startPort.direction &&
        //             port.node != startPort.node &&
        //             port.orientation == startPort.orientation &&
        //             port.portType == startPort.portType;
        //         var isCompatibleSyntaxPorts =
        //             port.orientation == Orientation.Horizontal &&
        //             port.direction != startPort.direction &&
        //             port.node != startPort.node &&
        //             port.orientation == startPort.orientation &&
        //             port.portType != null && startPort.portType != null &&
        //             (port.portType.IsAssignableFrom(startPort.portType) || startPort.portType.IsAssignableFrom(port.portType));
        //         if (isCompatibleBehaviorPorts || isCompatibleSyntaxPorts) compatiblePorts.Add(port);
        //     });
        //     return compatiblePorts;
        // }
        //

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.StopPropagation();
            var menuPosition = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            var context = new GenericMenu();

            FillSelectionActions();

            var popup = GenericMenuPopup.Get(context, "");
            popup.showSearch = true;
            popup.showTooltip = false;
            popup.resizeToContent = true;
            popup.Show(evt.mousePosition);

            void FillSelectionActions()
            {
                if (selection != null && selection.Any())
                {
                    context.AddItem(new GUIContent("Delete"), false, () => DeleteSelection());
                    context.AddSeparator("");
                }
            }
        // }
        //
        // // TODO: optimize?
        // private BehaviorNodeView CreateNode(IGraphNode node)
        // {
        //     var nodeView = new NodeView(node);
        //     _nodes.Add(nodeView.Id, nodeView);
        //     AddElement(nodeView);
        //     foreach (var child in node.Children)
        //     {
        //         var childView = CreateBehaviorNode(child);
        //         var edge = nodeView.Output.ConnectTo(childView.Input);
        //         AddElement(edge);
        //     }
        //     return nodeView;
        // }
        //
        // private void CreateVariantSyntaxEdge(IConnectableVariantViewContainer container)
        // {
        //     foreach (var view in container.Views.Where(v => v.Variant.IsConnected))
        //     {
        //         var syntaxNode = (SyntaxNodeView)_nodes[view.Variant.NodeId];
        //         syntaxNode.Connect(view.Variant, view.Variant.VariantPortIndex, view.Variant.SyntaxNodePortIndex);
        //         var edge = view.Ports[view.Variant.VariantPortIndex].ConnectTo(syntaxNode.Ports[view.Variant.SyntaxNodePortIndex]);
        //         AddElement(edge);
        //     }
        }
    }
}