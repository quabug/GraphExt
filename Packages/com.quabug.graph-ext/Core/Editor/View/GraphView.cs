using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Shtif;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;
using Node = UnityEditor.Experimental.GraphView.Node;

namespace GraphExt.Editor
{
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView, ITickableElement
    {
        public delegate void NodeEvent(in NodeId nodeId, Node node);
        public delegate void EdgeEvent(in EdgeId edgeId, Edge edge);
        public delegate void PortEvent(in PortId nodeId, Port node);

        public event NodeEvent OnNodeCreated;
        public event NodeEvent OnNodeWillDelete;

        public event EdgeEvent OnEdgeCreated;
        public event EdgeEvent OnEdgeWillDelete;

        public event PortEvent OnPortCreated;
        public event PortEvent OnPortWillDelete;

        public event NodeEvent OnNodeSelected;
        public event NodeEvent OnNodeUnselected;

        [NotNull] public GraphConfig Config { get; }
        [NotNull] public IGraphViewModule Module { get; set; } = new EmptyGraphViewModule();

        [NotNull] private readonly GraphElements<NodeId, NodeData, Node> _nodes;
        [NotNull] private readonly GraphElements<PortId, PortData, Port> _ports;
        [NotNull] private readonly GraphElements<EdgeId, EdgeId, Edge> _edges;

        public Node this[in NodeId nodeId] => _nodes.Elements[nodeId];
        public Port this[in PortId portId] => _ports.Elements[portId];
        public Edge this[in EdgeId edgeId] => _edges.Elements[edgeId];

        private readonly HashSet<NodeId> _selectedNodes = new HashSet<NodeId>();

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

            _nodes = new GraphElements<NodeId, NodeData, Node>(CreateNodeView, RemoveNodeView);
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
                foreach (var edge in @event.elementsToRemove.OfType<Edge>()) OnEdgeViewDeleted(edge);

                foreach (var nodeId in @event.elementsToRemove.OfType<Node>()
                    .Where(node => _nodes.Elements.ContainsValue(node))
                    .Select(node => _nodes.Elements.GetKey(node))
                ) Module.DeleteNode(nodeId);
            }

            if (@event.edgesToCreate != null)
            {
                foreach (var edge in @event.edgesToCreate) OnEdgeViewCreated(edge);
            }

            if (@event.movedElements != null)
            {
                foreach (var nodeView in @event.movedElements.OfType<Node>())
                {
                    if (_nodes.Elements.TryGetKey(nodeView, out var nodeId))
                    {
                        var position = nodeView.GetVector2Position();
                        Module.SetNodePosition(nodeId, position.x, position.y);
                    }
                }
            }

            return @event;
        }

        void OnEdgeViewCreated(Edge edge)
        {
            var inputId = _ports.Elements.GetKey(edge.input);
            var outputId = _ports.Elements.GetKey(edge.output);
            var edgeId = new EdgeId(inputId, outputId);
            if (!_edges.Elements.ContainsKey(edgeId))
            {
                Config.EdgeViewFactory.AfterCreated(edge);
                _edges.Elements.Add(edgeId, edge);
                Module.Connect(input: inputId, output: outputId);
                OnEdgeCreated?.Invoke(edgeId, edge);
            }
        }

        void OnEdgeViewDeleted(Edge edge)
        {
            var inputId = _ports.Elements.GetKey(edge.input);
            var outputId = _ports.Elements.GetKey(edge.output);
            var edgeId = new EdgeId(inputId, outputId);
            if (_edges.Elements.ContainsKey(edgeId))
            {
                OnEdgeWillDelete?.Invoke(edgeId, edge);
                _edges.Elements.Remove(edgeId);
                Module.Disconnect(input: inputId, output: outputId);
            }
        }

        public void Tick()
        {
            _nodes.UpdateElements(Module.NodeMap.Select(t => (t.id, t.data)));
            _ports.UpdateElements(Module.PortMap.Select(t => (t.id, t.data)));
            _edges.UpdateElements(Module.Edges.Select(edge => (edge, edge)));
            SendNodeSelection();
        }

        void SendNodeSelection()
        {
            foreach (var pair in _nodes.Elements)
            {
                var nodeId = pair.Key;
                var node = pair.Value;
                if (node.selected && !_selectedNodes.Contains(nodeId))
                {
                    _selectedNodes.Add(nodeId);
                    OnNodeSelected?.Invoke(nodeId, node);
                }
                else if (!node.selected && _selectedNodes.Contains(nodeId))
                {
                    _selectedNodes.Remove(nodeId);
                    OnNodeUnselected?.Invoke(nodeId, node);
                }
            }
        }

        [CanBeNull] private PortContainer FindPortContainer(PortId portId)
        {
            return _nodes.Elements.TryGetValue(portId.NodeId, out var node) ? node.Q<PortContainer>(name: portId.Name) : null;
        }

        private Node CreateNodeView(NodeId id, NodeData data)
        {
            if (!_nodes.Elements.ContainsKey(id))
            {
                var node = Config.NodeViewFactory.Create(data);
                node.name = id.ToString();
                AddElement(node);
                OnNodeCreated?.Invoke(id, node);
                return node;
            }
            return null;
        }

        private void RemoveNodeView(NodeId id)
        {
            if (_nodes.Elements.TryGetValue(id, out var node))
            {
                if (_selectedNodes.Contains(id))
                {
                    _selectedNodes.Remove(id);
                    OnNodeUnselected?.Invoke(id, node);
                }
                OnNodeWillDelete?.Invoke(id, node);
                RemoveElement(node);
            }
        }

        [CanBeNull] private Port CreatePortView(PortId id, PortData data)
        {
            var container = FindPortContainer(id);
            if (container != null)
            {
                var port = Config.PortViewFactory.CreatePort(data);
                container.AddPort(port);
                OnPortCreated?.Invoke(id, port);
                return port;
            }
            return null;
        }

        private void RemovePortView(PortId id)
        {
            var container = FindPortContainer(id);
            container?.RemovePort().DisconnectAll();
        }

        private Edge CreateEdgeView(EdgeId id, EdgeId _)
        {
            if (_edges.Elements.ContainsKey(id)) return null;

            _ports.Elements.TryGetValue(id.Input, out var inputPortView);
            _ports.Elements.TryGetValue(id.Output, out var outputPortView);
            if (inputPortView != null && outputPortView != null)
            {
                var edge = Config.EdgeViewFactory.CreateEdge(inputPortView, outputPortView);
                AddElement(edge);
                OnEdgeCreated?.Invoke(id, edge);
                return edge;
            }
            return null;
        }

        private void RemoveEdgeView(EdgeId id)
        {
            if (_edges.Elements.TryGetValue(id, out var edge))
            {
                OnEdgeWillDelete?.Invoke(id, edge);
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                RemoveElement(edge);
            }
        }
    }
}