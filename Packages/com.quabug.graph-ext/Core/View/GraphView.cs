using System.Linq;
using BinaryEgo.Editor.UI;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView, ITickableElement
    {
        private readonly GraphConfig _config;
        public IGraphModule Module { get; internal set; }
        // private IDictionary<int, Node> _nodes = new Dictionary<int, Node>();

        public GraphView(GraphConfig config)
        {
            _config = config;
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

            graphViewChanged += OnGraphChanged;
        }

        private void CreateNode(INodeModule node)
        {
            var nodeView = new NodeView(node);
            AddElement(nodeView);
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
            // var menuPosition = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            var context = new GenericMenu();
            foreach (var menu in _config.Menu) menu.MakeEntry(this, evt, context);
            var popup = GenericMenuPopup.Get(context, "");
            popup.showSearch = true;
            popup.showTooltip = false;
            popup.resizeToContent = true;
            popup.Show(evt.mousePosition);
        }

        private GraphViewChange OnGraphChanged(GraphViewChange @event)
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
        public void Tick()
        {
            // _nodes.Clear();
            DeleteElements(graphElements.ToList());

            if (Module != null)
            {
                foreach (var node in Module.Nodes) CreateNode(node);
                // foreach (var node in _nodes.Values.Cast<IConnectableVariantViewContainer>()) CreateEdges(node);
            }
        }
    }
}