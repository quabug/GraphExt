using System.Collections.Generic;
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
        [NotNull] public GraphConfig Config { get; }
        [NotNull] public IGraphModule Module { get; set; }

        private readonly BiDictionary<INodeModule, Node> _nodes = new BiDictionary<INodeModule, Node>();
        private readonly BiDictionary<EdgeData, Edge> _edges = new BiDictionary<EdgeData, Edge>();

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
                foreach (var node in @event.elementsToRemove.OfType<Node>()
                    .Where(_nodes.ContainsValue)
                    .Select(_nodes.GetKey)
                ) node.Dispose();
            }

            if (@event.edgesToCreate != null)
            {
                foreach (var edge in @event.edgesToCreate) OnEdgeCreated(edge, ref @event);
            }

            if (@event.movedElements != null)
            {
                foreach (var (view, module) in
                    from view in @event.movedElements.OfType<Node>()
                    where _nodes.ContainsValue(view)
                    select (view, module: _nodes.GetKey(view))
                ) module.Position = view.GetPosition().position;
            }

            return @event;
        }

        void OnEdgeCreated(Edge edge, ref GraphViewChange @event)
        {
            edge.showInMiniMap = true;
            if (edge.input is PortView inputPort && edge.output is PortView outputPort)
            {
                Module.Connect(inputPort.Module, outputPort.Module);
            }
        }

        void OnEdgeDeleted(Edge edge)
        {
            if (edge.input is PortView inputPort && edge.output is PortView outputPort)
            {
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

        }

        void RefreshNodes()
        {
            var currentNodes = new HashSet<INodeModule>(_nodes.Keys);
            foreach (var node in Module.Nodes)
            {
                if (currentNodes.Contains(node)) currentNodes.Remove(node);
                else CreateNode(node);
            }
            foreach (var removed in currentNodes) RemoveNode(removed);

            void CreateNode(INodeModule node)
            {
                var nodeView = new NodeView(node, Config);
                _nodes.Add(node, nodeView);
                AddElement(nodeView);
            }

            void RemoveNode(INodeModule node)
            {
                RemoveElement(_nodes[node]);
                _nodes.Remove(node);
            }
        }
    }
}