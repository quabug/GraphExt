using System;
using System.Collections.Generic;
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
        [NotNull] private readonly ConvertToNodeData _nodeConvertor;
        [NotNull] private readonly Func<IEnumerable<NodeId>> _newNodes;
        [NotNull] private readonly IBiDictionary<NodeId, Node> _currentNodeViews;
        [NotNull] private readonly IBiDictionary<PortId, Port> _currentPortViews;
        [NotNull] private readonly IDictionary<PortId, PortData> _currentPortDataMap;

        public NodeViewPresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView view,
            [NotNull] INodeViewFactory nodeViewFactory,
            [NotNull] IPortViewFactory portViewFactory,
            [NotNull] ConvertToNodeData nodeConvertor,
            [NotNull] Func<IEnumerable<NodeId>> newNodes,
            [NotNull] IBiDictionary<NodeId, Node> currentNodeViews,
            [NotNull] IBiDictionary<PortId, Port> currentPortViews,
            [NotNull] IDictionary<PortId, PortData> currentPortDataMap
        )
        {
            _view = view;
            _nodeViewFactory = nodeViewFactory;
            _portViewFactory = portViewFactory;
            _nodeConvertor = nodeConvertor;
            _newNodes = newNodes;
            _currentNodeViews = currentNodeViews;
            _currentPortViews = currentPortViews;
            _currentPortDataMap = currentPortDataMap;
        }

        public void Tick()
        {
            var newNodes = _newNodes().ToDictionary(node => node, node => _nodeConvertor(node));
            UpdateNodes();
            UpdatePorts();

            void UpdateNodes()
            {
                var (added, removed) = _currentNodeViews.Select(t => t.Key).Diff(newNodes.Keys);

                foreach (var node in added)
                {
                    var nodeView = _nodeViewFactory.Create(_nodeConvertor(node));
                    _currentNodeViews.Add(node, nodeView);
                    _view.AddElement(nodeView);
                }

                foreach (var node in removed)
                {
                    var nodeView = _currentNodeViews[node];
                    _view.RemoveElement(nodeView);
                    _currentNodeViews.Remove(node);
                }
            }

            void UpdatePorts()
            {
                var newPorts = newNodes
                    .SelectMany(node => node.Value.Ports.Select(port => (portId: new PortId(node.Key, port.Key), port: port.Value)))
                    .ToDictionary(t => t.portId, t => t.port)
                ;
                var (added, removed) = _currentPortViews.Keys.Diff(newPorts.Keys);

                foreach (var port in added)
                {
                    var container = FindPortContainer(port);
                    if (container == null) continue;
                    var nodeView = newNodes[port.NodeId];
                    var portView = _portViewFactory.CreatePort(nodeView.Ports[port.Name]);
                    _currentPortViews.Add(port, portView);
                    _currentPortDataMap.Add(port, newPorts[port]);
                    container.AddPort(portView);
                }

                foreach (var port in removed)
                {
                    var portView = _currentPortViews[port];
                    portView.DisconnectAll();
                    _currentPortViews.Remove(port);
                    _currentPortDataMap.Remove(port);
                    var container = FindPortContainer(port);
                    container?.RemovePort();
                }
            }

            PortContainer FindPortContainer(PortId portId)
            {
                if (!_currentNodeViews.ContainsKey(portId.NodeId)) return null;
                return _currentNodeViews[portId.NodeId].Query<PortContainer>()
                    .Where(container => container.PortName == portId.Name)
                    .First()
                ;
            }
        }
    }
}