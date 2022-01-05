using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt.Editor
{
    public interface IGraphViewModule
    {
        [NotNull] IEnumerable<(PortId id, PortData data)> Ports { get; }
        [NotNull] IEnumerable<(NodeId id, NodeData data)> Nodes { get; }
        [NotNull] IEnumerable<EdgeId> Edges { get; }

        void DeleteNode(in NodeId nodeId);
        void SetNodePosition(in NodeId nodeId, float x, float y);
        bool IsCompatible(in PortId input, in PortId output);
        void Connect(in PortId input, in PortId output);
        void Disconnect(in PortId input, in PortId output);
    }

    public class EmptyGraphViewModule : IGraphViewModule
    {
        public IEnumerable<(PortId id, PortData data)> Ports => Enumerable.Empty <(PortId, PortData)> ();
        public IEnumerable<(NodeId id, NodeData data)> Nodes => Enumerable.Empty<(NodeId id, NodeData data)>();
        public IEnumerable<EdgeId> Edges => Enumerable.Empty<EdgeId>();
        public void DeleteNode(in NodeId nodeId) {}
        public void SetNodePosition(in NodeId nodeId, float x, float y) {}
        public bool IsCompatible(in PortId input, in PortId output) => true;
        public void Connect(in PortId input, in PortId output) {}
        public void Disconnect(in PortId input, in PortId output) {}
    }

    public abstract class GraphViewModule<TNode> : IGraphViewModule where TNode : INode<GraphRuntime<TNode>>
    {
        public abstract GraphRuntime<TNode> Runtime { get; }

        public IEnumerable<(PortId id, PortData data)> Ports
        {
            get
            {
                _PortData.Clear();
                foreach (var nodeId in _NodeData.Keys)
                {
                    var ports = FindNodePorts(nodeId);
                    _PortData[nodeId] = ports;
                }
                return from nodePorts in _PortData
                    from port in nodePorts.Value
                    select (new PortId(nodePorts.Key, port.Key), port.Value)
                ;
            }
        }

        public IEnumerable<(NodeId id, NodeData data)> Nodes
        {
            get
            {
                var removed = new HashSet<NodeId>(_NodeData.Keys);
                foreach (var nodeId in Runtime.NodeMap.Keys)
                {
                    if (_NodeData.ContainsKey(nodeId))
                    {
                        removed.Remove(nodeId);
                    }
                    else
                    {
                        _PortData[nodeId] = FindNodePorts(nodeId);
                        _NodeData[nodeId] = ToNodeData(nodeId);
                    }
                }

                foreach (var nodeId in removed)
                {
                    _PortData.Remove(nodeId);
                    _NodeData.Remove(nodeId);
                }

                return _NodeData.Select(pair => (pair.Key, pair.Value));
            }
        }

        public IEnumerable<EdgeId> Edges => Runtime.Edges;

        protected readonly Dictionary<NodeId, NodeData> _NodeData = new Dictionary<NodeId, NodeData>();
        protected readonly Dictionary<NodeId, IReadOnlyDictionary<string, PortData>> _PortData =
            new Dictionary<NodeId, IReadOnlyDictionary<string, PortData>>();

        protected void AddNode(in NodeId nodeId, TNode node)
        {
            AddNode(nodeId, node, 0, 0);
        }

        protected void AddNode(in NodeId nodeId, TNode node, float x, float y)
        {
            Runtime.AddNode(nodeId, node);
            _PortData[nodeId] = FindNodePorts(nodeId);
            SetNodePosition(nodeId, x, y);
            _NodeData[nodeId] = ToNodeData(nodeId);
        }

        public virtual void DeleteNode(in NodeId nodeId)
        {
            _NodeData.Remove(nodeId);
            _PortData.Remove(nodeId);
            Runtime.DeleteNode(nodeId);
        }

        public virtual void SetNodePosition(in NodeId nodeId, float x, float y) {}

        public virtual bool IsCompatible(in PortId input, in PortId output)
        {
            var inputPort = _PortData[input.NodeId][input.Name];
            var outputPort = _PortData[output.NodeId][output.Name];
            return inputPort.Direction != outputPort.Direction &&
                   inputPort.Orientation == outputPort.Orientation &&
                   // single port could be handled by Unity Graph
                   (inputPort.Capacity == 1 || FindConnections(input).Count() < inputPort.Capacity) &&
                   (outputPort.Capacity == 1 || FindConnections(output).Count() < outputPort.Capacity) &&
                   Runtime.GetNodeByPort(input).IsPortCompatible(Runtime, input, output) &&
                   Runtime.GetNodeByPort(output).IsPortCompatible(Runtime, input, output)
            ;
        }

        protected IEnumerable<EdgeId> FindConnections(PortId portId)
        {
            return Runtime.Edges.Where(edge => edge.Contains(portId));
        }

        public virtual void Connect(in PortId input, in PortId output)
        {
            Runtime.Connect(input, output);
        }

        public virtual void Disconnect(in PortId input, in PortId output)
        {
            Runtime.Disconnect(input, output);
        }

        [NotNull] protected abstract IReadOnlyDictionary<string/*portName*/, PortData> FindNodePorts(in NodeId nodeId);
        protected abstract NodeData ToNodeData(in NodeId nodeId);
    }
}