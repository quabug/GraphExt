using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeViewPresenter : IViewPresenter
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _view;
        [NotNull] private readonly INodeViewFactory _nodeViewFactory;
        [NotNull] private readonly IPortViewFactory _portViewFactory;
        [NotNull] private readonly INodesViewModule _nodesViewModule;
        [NotNull] private readonly GraphElements<NodeId, Node> _nodes;
        [NotNull] private readonly GraphElements<PortId, Port> _ports;

        public NodeViewPresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView view,
            [NotNull] INodeViewFactory nodeViewFactory,
            [NotNull] IPortViewFactory portViewFactory,
            [NotNull] INodesViewModule nodesViewModule,
            [NotNull] GraphElements<NodeId, Node> nodes,
            [NotNull] GraphElements<PortId, Port> ports
        )
        {
            _view = view;
            _nodeViewFactory = nodeViewFactory;
            _portViewFactory = portViewFactory;
            _nodesViewModule = nodesViewModule;
            _nodes = nodes;
            _ports = ports;
        }

        public void Tick()
        {
            var newNodes = _nodesViewModule.GetNodes();
            UpdateNodes();
            UpdatePorts();

            void UpdateNodes()
            {
                var (added, removed) = _nodes.Ids.Diff(newNodes.Keys);

                foreach (var node in added)
                {
                    var nodeView = _nodeViewFactory.Create(newNodes[node]);
                    _nodes.Add(node, nodeView);
                    _view.AddElement(nodeView);
                }

                foreach (var node in removed)
                {
                    var nodeView = _nodes[node];
                    _view.RemoveElement(nodeView);
                    _nodes.Remove(node);
                }
            }

            void UpdatePorts()
            {
                var newPorts = newNodes.SelectMany(node => node.Value.Ports.Select(port => new PortId(node.Key, port.Key)));
                var (added, removed) = _ports.Ids.Diff(newPorts);

                foreach (var port in added)
                {
                    var container = FindPortContainer(port);
                    if (container == null) continue;
                    var nodeView = newNodes[port.NodeId];
                    var portView = _portViewFactory.CreatePort(nodeView.Ports[port.Name]);
                    _ports.Add(port, portView);
                    container.AddPort(portView);
                }

                foreach (var port in removed)
                {
                    var container = FindPortContainer(port);
                    if (container == null) continue;
                    container.RemovePort();
                    _ports.Remove(port);
                }
            }

            PortContainer FindPortContainer(PortId portId)
            {
                return _nodes[portId.NodeId].Query<PortContainer>()
                    .Where(container => container.PortName == portId.Name)
                    .First()
                ;
            }
        }
    }
}