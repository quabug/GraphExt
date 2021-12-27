using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public interface IGraphViewModule
    {
        [NotNull] IEnumerable<(PortId id, PortData data)> PortMap { get; }
        [NotNull] IEnumerable<(NodeId id, NodeData data)> NodeMap { get; }
        [NotNull] IEnumerable<EdgeId> Edges { get; }

        void DeleteNode(in NodeId nodeId);
        bool IsCompatible(in PortId input, in PortId output);
        void Connect(in PortId input, in PortId output);
        void Disconnect(in PortId input, in PortId output);
    }

    public abstract class GraphViewModule<TNode> : IGraphViewModule where TNode : INode<GraphRuntime<TNode>>
    {
        public abstract GraphRuntime<TNode> Runtime { get; }

        public IEnumerable<(PortId id, PortData data)> PortMap => _portDataCache.Select(pair => (pair.Key, pair.Value));
        public IEnumerable<(NodeId id, NodeData data)> NodeMap => _nodeDataCache.Select(pair => (pair.Key, pair.Value));
        public IEnumerable<EdgeId> Edges => Runtime.Edges;

        private readonly Dictionary<NodeId, NodeData> _nodeDataCache = new Dictionary<NodeId, NodeData>();
        private readonly Dictionary<PortId, PortData> _portDataCache = new Dictionary<PortId, PortData>();

        protected void AddNode(in NodeId nodeId, TNode node)
        {
            _nodeDataCache[nodeId] = ToNodeData(nodeId, node);
            var ports = FindNodePorts(node).ToArray();
            foreach (var port in ports) _portDataCache[new PortId(nodeId, port.Name)] = port;
            Runtime.AddNode(nodeId, node);
        }

        public virtual void DeleteNode(in NodeId nodeId)
        {
            _nodeDataCache.Remove(nodeId);
            var id = nodeId;
            _portDataCache.RemoveWhere(port => port.Key.NodeId == id);
            Runtime.DeleteNode(nodeId);
        }

        public virtual bool IsCompatible(in PortId input, in PortId output)
        {
            var inputPort = _portDataCache[input];
            var outputPort = _portDataCache[output];
            return inputPort.Direction != outputPort.Direction &&
                   inputPort.Orientation == outputPort.Orientation &&
                   Runtime.IsCompatible(input, output)
            ;
        }

        public virtual void Connect(in PortId input, in PortId output)
        {
            Runtime.Connect(input, output);
        }

        public virtual void Disconnect(in PortId input, in PortId output)
        {
            Runtime.Disconnect(input, output);
        }

        [NotNull] protected abstract IEnumerable<PortData> FindNodePorts([NotNull] TNode node);
        protected abstract NodeData ToNodeData(in NodeId nodeId, [NotNull] TNode node);
    }

    public interface INodeProperty {}

    public readonly struct NodeData
    {
        [NotNull] public readonly IReadOnlyList<INodeProperty> Properties;
        public NodeData([NotNull] IReadOnlyList<INodeProperty> properties) => Properties = properties;
    }

    public readonly struct PortData
    {
        public readonly string Name;
        public readonly Orientation Orientation;
        public readonly Direction Direction;
        public readonly Port.Capacity Capacity;
        public readonly Type PortType;

        public PortData(string name, Orientation orientation, Direction direction, Port.Capacity capacity, Type portType)
        {
            Name = name;
            Orientation = orientation;
            Direction = direction;
            Capacity = capacity;
            PortType = portType;
        }
    }
}