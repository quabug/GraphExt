using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryEgo.Editor.UI;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView, ITickableElement
    {
        private static string _defaultNodeUi = Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml");

        [NotNull] public GraphConfig Config { get; }
        [NotNull] public IGraphModule Module { get; set; }

        private readonly BiDictionary<Guid, Node> _nodes = new BiDictionary<Guid, Node>();
        private readonly BiDictionary<EdgeId, Edge> _edges = new BiDictionary<EdgeId, Edge>();

        public GraphView([NotNull] GraphConfig config)
        {
            Config = config;
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

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            if (startPort is PortView startPortView)
            {
                ports.ForEach(endPort =>
                {
                    if (endPort is PortView endPortView && Module.IsCompatible(startPortView.Module, endPortView.Module))
                        compatiblePorts.Add(endPort);
                });
            }
            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.StopPropagation();
            var context = new GenericMenu();
            foreach (var menu in Config.Menu) menu.MakeEntry(this, evt, context);
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
                foreach (var nodeId in @event.elementsToRemove.OfType<Node>()
                    .Where(_nodes.ContainsValue)
                    .Select(_nodes.GetKey)
                ) Module.DeleteNode(nodeId);
            }

            if (@event.edgesToCreate != null)
            {
                foreach (var edge in @event.edgesToCreate) OnEdgeCreated(edge, ref @event);
            }

            if (@event.movedElements != null)
            {
                foreach (var (nodeView, nodeId) in
                    from nodeView in @event.movedElements.OfType<Node>()
                    where _nodes.ContainsValue(nodeView)
                    select (nodeView, nodeId: _nodes.GetKey(nodeView))
                ) Module.SetNodePosition(nodeId, nodeView.GetPosition().position);
            }

            return @event;
        }

        void OnEdgeCreated(Edge edge, ref GraphViewChange @event)
        {
            edge.showInMiniMap = true;
            if (edge.input is PortView inputPort && edge.output is PortView outputPort)
            {
                _edges.Add(new EdgeId(outputPort.Id, inputPort.Id), edge);
                Module.Connect(inputPort.Module, outputPort.Module);
            }
        }

        void OnEdgeDeleted(Edge edge)
        {
            if (edge.input is PortView inputPort && edge.output is PortView outputPort)
            {
                _edges.Remove(new EdgeId(outputPort.Id, inputPort.Id));
                Module.Disconnect(inputPort.Module, outputPort.Module);
            }
        }

        public void Tick()
        {
            RefreshNodes();
            RefreshEdges();
        }

        void RefreshEdges()
        {
            // var currentEdges = new HashSet<EdgeId>(_edges.Keys);
            // foreach (var edge in Module.Edges)
            // {
            //     if (currentEdges.Contains(edge)) currentEdges.Remove(edge);
            //     else CreateEdge(edge);
            // }
            // foreach (var removed in currentNodes) RemoveNode(removed);
            //
            // void CreateEdge(EdgeId edge)
            // {
            //     var nodeView = new NodeView(node, Config);
            //     _nodes.Add(node, nodeView);
            //     AddElement(nodeView);
            // }
            //
            // void RemoveNode(INodeModule node)
            // {
            //     RemoveElement(_nodes[node]);
            //     _nodes.Remove(node);
            // }
        }

        void RefreshNodes()
        {
            var currentNodes = new HashSet<Guid>(_nodes.Keys);
            foreach (var node in Module.Nodes)
            {
                if (currentNodes.Contains(node.Id)) currentNodes.Remove(node.Id);
                else CreateNode(node);
            }
            foreach (var removed in currentNodes) RemoveNode(removed);

            void CreateNode(INodeModule node)
            {
                var nodeView = new Node(node.UiFile ?? _defaultNodeUi);
                var container = nodeView.ContentContainer();
                foreach (var property in node.Properties)
                {
                    var propertyView = Config.CreatePropertyView(property);
                    container.Add(propertyView);
                }
                AddElement(nodeView);
                nodeView.SetPosition(node.Position);
                _nodes.Add(node.Id, nodeView);
            }

            void RemoveNode(Guid nodeId)
            {
                RemoveElement(_nodes[nodeId]);
                _nodes.Remove(nodeId);
            }
        }
    }
}