using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryEgo.Editor.UI;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;
using Node = UnityEditor.Experimental.GraphView.Node;

namespace GraphExt
{
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView, ITickableElement
    {
        private static readonly string _defaultNodeUi = Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml");

        [NotNull] public GraphConfig Config { get; }
        [NotNull] public IGraph Module { get; set; }

        [NotNull] private readonly GraphElements<NodeId, INodeData, Node> _nodes;
        [NotNull] private readonly GraphElements<PortId, PortData, Port> _ports;
        [NotNull] private readonly GraphElements<EdgeId, EdgeId, Edge> _edges;

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

            _nodes = new GraphElements<NodeId, INodeData, Node>(CreateNodeView, RemoveNodeView);
            _ports = new GraphElements<PortId, PortData, Port>(CreatePortView, RemovePortView);
            _edges = new GraphElements<EdgeId, EdgeId, Edge>(CreateEdgeView, RemoveEdgeView);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(endPort =>
            {
                if (Module.IsCompatible(output: _ports.Elements.GetKey(startPort), input: _ports.Elements.GetKey(endPort)))
                    compatiblePorts.Add(endPort);
            });
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

        public NodeId GetNodeId(Node node) => _nodes.Elements.GetKey(node);
        public PortId GetPortId(Port port) => _ports.Elements.GetKey(port);
        public EdgeId GetEdgeId(Edge edge) => _edges.Elements.GetKey(edge);

        private GraphViewChange OnGraphChanged(GraphViewChange @event)
        {
            if (@event.elementsToRemove != null)
            {
                foreach (var edge in @event.elementsToRemove.OfType<Edge>()) OnEdgeDeleted(edge);

                foreach (var nodeId in @event.elementsToRemove.OfType<Node>()
                    .Where(node => _nodes.Elements.ContainsValue(node))
                    .Select(node => _nodes.Elements.GetKey(node))
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
                    where _nodes.Elements.ContainsValue(nodeView)
                    select (nodeView, nodeId: _nodes.Elements.GetKey(nodeView))
                ) Module.SetNodePosition(nodeId, nodeView.GetPosition().position);
            }

            return @event;
        }

        void OnEdgeCreated(Edge edge, ref GraphViewChange @event)
        {
            edge.showInMiniMap = true;
            var inputId = _ports.Elements.GetKey(edge.input);
            var outputId = _ports.Elements.GetKey(edge.output);
            var edgeId = new EdgeId(inputId, outputId);
            if (!_edges.Elements.ContainsKey(edgeId))
            {
                _edges.Elements.Add(edgeId, edge);
                Module.Connect(input: inputId, output: outputId);
            }
        }

        void OnEdgeDeleted(Edge edge)
        {
            var inputId = _ports.Elements.GetKey(edge.input);
            var outputId = _ports.Elements.GetKey(edge.output);
            var edgeId = new EdgeId(inputId, outputId);
            if (_edges.Elements.ContainsKey(edgeId))
            {
                _edges.Elements.Remove(edgeId);
                Module.Disconnect(input: inputId, output: outputId);
            }
        }

        public void Tick()
        {
            _nodes.UpdateElements(Module.Nodes.Select(node => (node.Id, n: node)));
            _ports.UpdateElements(Module.Ports.Select(port => (port.Id, p: port)));
            _edges.UpdateElements(Module.Edges.Select(edge => (edge, edge)));
        }

        [CanBeNull] private PortContainer FindPortContainer(PortId portId)
        {
            return _nodes.Elements.TryGetValue(portId.NodeId, out var node) ? node.Query<PortContainer>().Where(p => p.PortId == portId).First() : null;
        }

        private Node CreateNodeView(INodeData data)
        {
            var nodeView = new Node(data.UiFile ?? _defaultNodeUi);
            var container = nodeView.ContentContainer();
            foreach (var property in data.Properties)
            {
                var propertyView = Config.CreatePropertyView(property);
                container.Add(propertyView);
            }
            AddElement(nodeView);
            nodeView.SetPosition(data.Position);
            return nodeView;
        }

        private void RemoveNodeView(NodeId id)
        {
            if (_nodes.Elements.TryGetValue(id, out var node))
            {
                RemoveElement(node);
            }
        }

        [CanBeNull] private Port CreatePortView(PortData data)
        {
            var container = FindPortContainer(data.Id);
            if (container != null)
            {
                var port = Port.Create<Edge>(data.Orientation, data.Direction, data.Capacity, data.PortType);
                port.style.paddingLeft = 0;
                port.style.paddingRight = 0;
                container.AddPort(port);
                return port;
            }
            return null;
        }

        private void RemovePortView(PortId id)
        {
            var container = FindPortContainer(id);
            container?.RemovePort().DisconnectAll();
        }

        private Edge CreateEdgeView(EdgeId data)
        {
            if (_edges.Elements.ContainsKey(data)) return null;

            var port1 = _ports.Elements[data.First];
            var port2 = _ports.Elements[data.Second];
            var edge = port1.ConnectTo(port2);
            AddElement(edge);
            return edge;
        }

        private void RemoveEdgeView(EdgeId id)
        {
            if (_edges.Elements.TryGetValue(id, out var edge))
            {
                RemoveElement(edge);
            }
        }
    }
}