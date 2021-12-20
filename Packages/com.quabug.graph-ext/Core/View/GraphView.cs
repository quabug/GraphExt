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

        private readonly BiDictionary<NodeId, Node> _nodes = new BiDictionary<NodeId, Node>();
        private readonly BiDictionary<PortId, Port> _ports = new BiDictionary<PortId, Port>();
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
            ports.ForEach(endPort =>
            {
                if (Module.IsCompatible(output: _ports.GetKey(startPort), input: _ports.GetKey(endPort)))
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

        private GraphViewChange OnGraphChanged(GraphViewChange @event)
        {
            if (@event.elementsToRemove != null)
            {
                foreach (var edge in @event.elementsToRemove.OfType<Edge>()) OnEdgeDeleted(edge);
                foreach (var nodeId in @event.elementsToRemove.OfType<Node>()
                    .Where(node => _nodes.ContainsValue(node))
                    .Select(node => _nodes.GetKey(node))) Module.DeleteNode(nodeId);
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
            var inputId = _ports.GetKey(edge.input);
            var outputId = _ports.GetKey(edge.output);
            _edges.Add(new EdgeId(inputId, outputId), edge);
            Module.Connect(input: inputId, output: outputId);
        }

        void OnEdgeDeleted(Edge edge)
        {
            var inputId = _ports.GetKey(edge.input);
            var outputId = _ports.GetKey(edge.output);
            _edges.Remove(new EdgeId(inputId, outputId));
            Module.Disconnect(input: inputId, output: outputId);
        }

        public void Tick()
        {
            RefreshNodes();
            RefreshPorts();
            RefreshEdges();
        }

        void RefreshPorts()
        {
            var currentPorts = new HashSet<PortId>(_ports.Keys);

            foreach (var port in Module.Ports)
            {
                if (currentPorts.Contains(port.Id)) currentPorts.Remove(port.Id);
                else CreatePort(port);
            }
            foreach (var removed in currentPorts) RemovePort(removed);

            void CreatePort(IPortModule port)
            {
                var container = FindPortContainer(port.Id);
                if (container == null) return; // log?
                var portView = Port.Create<Edge>(port.Orientation, port.Direction, port.Capacity, port.PortType);
                portView.style.paddingLeft = 0;
                portView.style.paddingRight = 0;
                container.AddPort(portView);
                _ports.Add(port.Id, portView);
            }

            void RemovePort(in PortId portId)
            {
                var container = FindPortContainer(portId);
                if (container == null) return; // log?
                container.RemovePort();
            }
        }

        PortContainer FindPortContainer(PortId portId)
        {
            var node = _nodes[portId.NodeId];
            return node.Query<PortContainer>().Where(p => p.PortId == portId).First();
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
            var currentNodes = new HashSet<NodeId>(_nodes.Keys);

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

            void RemoveNode(in NodeId nodeId)
            {
                RemoveElement(_nodes[nodeId]);
                _nodes.Remove(nodeId);
            }
        }
    }
}