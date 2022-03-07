using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeViewPresenter : IWindowSystem, IDisposable
    {
        public class NodeAddedEvent
        {
            public Action<NodeId> Event;
        }

        public class NodeDeletedEvent
        {
            public Action<NodeId> Event;
        }

        private readonly NodeAddedEvent _onNodeAdded;
        private readonly NodeDeletedEvent _onNodeDeleted;

        private readonly UnityEditor.Experimental.GraphView.GraphView _view;
        private readonly INodeViewFactory _nodeViewFactory;
        private readonly IPortViewFactory _portViewFactory;
        private readonly ConvertToNodeData _nodeConvertor;
        private readonly FindPortData _findPorts;
        private readonly IBiDictionary<NodeId, Node> _currentNodeViews;
        private readonly IBiDictionary<PortId, Port> _currentPortViews;
        private readonly IDictionary<PortId, PortData> _currentPortDataMap;

        public NodeViewPresenter(
            UnityEditor.Experimental.GraphView.GraphView view,
            INodeViewFactory nodeViewFactory,
            IPortViewFactory portViewFactory,
            IEnumerable<NodeId> initializeNodesProvider,
            NodeAddedEvent onNodeAdded,
            NodeDeletedEvent onNodeDeleted,
            ConvertToNodeData nodeConvertor,
            FindPortData findPorts,
            IBiDictionary<NodeId, Node> currentNodeViews,
            IBiDictionary<PortId, Port> currentPortViews,
            IDictionary<PortId, PortData> currentPortDataMap
        )
        {
            _view = view;
            _nodeViewFactory = nodeViewFactory;
            _portViewFactory = portViewFactory;
            _onNodeAdded = onNodeAdded;
            _onNodeDeleted = onNodeDeleted;
            _nodeConvertor = nodeConvertor;
            _findPorts = findPorts;
            _currentNodeViews = currentNodeViews;
            _currentPortViews = currentPortViews;
            _currentPortDataMap = currentPortDataMap;

            foreach (var node in initializeNodesProvider) AddNode(node);
            _onNodeAdded.Event += AddNode;
            _onNodeDeleted.Event += DeleteNode;
        }

        public void Dispose()
        {
            _onNodeAdded.Event -= AddNode;
            _onNodeDeleted.Event -= DeleteNode;
        }

        private void DeleteNode(NodeId nodeId)
        {
            DeleteNodePorts(nodeId);
            var nodeView = _currentNodeViews[nodeId];
            _view.RemoveElement(nodeView);
            _currentNodeViews.Remove(nodeId);
        }

        private void AddNode(NodeId nodeId)
        {
            var nodeView = _nodeViewFactory.Create(_nodeConvertor(nodeId));
            _currentNodeViews.Add(nodeId, nodeView);
            _view.AddElement(nodeView);
            AddNodePorts(nodeId);
        }

        private PortContainer FindPortContainer(PortId portId)
        {
            if (!_currentNodeViews.ContainsKey(portId.NodeId)) return null;
            return _currentNodeViews[portId.NodeId].Query<PortContainer>()
                .Where(container => container.PortName == portId.Name)
                .First()
            ;
        }

        private void AddNodePorts(NodeId nodeId)
        {
            var nodePorts = _findPorts(nodeId).Select(port => (portId: new PortId(nodeId, port.Name), port: port));
            foreach (var (portId, port) in nodePorts)
            {
                var container = FindPortContainer(portId);
                if (container == null) continue;
                var portView = _portViewFactory.CreatePort(port);
                _currentPortViews.Add(portId, portView);
                _currentPortDataMap.Add(portId, port);
                container.AddPort(portView);
            }
        }

        private void DeleteNodePorts(NodeId nodeId)
        {
            foreach (var port in _currentPortViews.Keys.Where(portId => portId.NodeId == nodeId).ToArray())
            {
                var portView = _currentPortViews[port];
                portView.DisconnectAll();
                _currentPortViews.Remove(port);
                _currentPortDataMap.Remove(port);
                var container = FindPortContainer(port);
                container?.RemovePort();
            }
        }
    }
}