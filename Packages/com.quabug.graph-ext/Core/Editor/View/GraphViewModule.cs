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
        void SetNodePosition(in NodeId nodeId, float x, float y);
        bool IsCompatible(in PortId input, in PortId output);
        void Connect(in PortId input, in PortId output);
        void Disconnect(in PortId input, in PortId output);
    }

    public abstract class GraphViewModule<TNode> : IGraphViewModule where TNode : INode<GraphRuntime<TNode>>
    {
        public abstract GraphRuntime<TNode> Runtime { get; }

        protected readonly Dictionary<NodeId, NodeData> _NodeMap = new Dictionary<NodeId, NodeData>();
        protected readonly Dictionary<PortId, PortData> _PortMap = new Dictionary<PortId, PortData>();

        public IEnumerable<(PortId id, PortData data)> PortMap => _PortMap.Select(pair => (pair.Key, pair.Value));
        public IEnumerable<(NodeId id, NodeData data)> NodeMap => _NodeMap.Select(pair => (pair.Key, pair.Value));
        public IEnumerable<EdgeId> Edges => Runtime.Edges;

        public virtual void AddNode(in NodeId nodeId, TNode node)
        {
            _NodeMap[nodeId] = ToNodeData(nodeId, node);
            var ports = FindNodePorts(node).ToArray();
            foreach (var port in ports) _PortMap[new PortId(nodeId, port.Name)] = port;
            Runtime.AddNode(nodeId, node);
        }

        public virtual void DeleteNode(in NodeId nodeId)
        {
            _NodeMap.Remove(nodeId);
            var id = nodeId;
            _PortMap.RemoveWhere(port => port.Key.NodeId == id);
            Runtime.DeleteNode(nodeId);
        }

        public virtual void SetNodePosition(in NodeId nodeId, float x, float y) {}

        public virtual bool IsCompatible(in PortId input, in PortId output)
        {
            var inputPort = _PortMap[input];
            var outputPort = _PortMap[output];
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