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

    public class EmptyGraphViewModule : IGraphViewModule
    {
        public IEnumerable<(PortId id, PortData data)> PortMap => Enumerable.Empty <(PortId, PortData)> ();
        public IEnumerable<(NodeId id, NodeData data)> NodeMap => Enumerable.Empty<(NodeId id, NodeData data)>();
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

        public IEnumerable<(PortId id, PortData data)> PortMap => _PortData.Select(pair => (pair.Key, pair.Value));
        public IEnumerable<(NodeId id, NodeData data)> NodeMap => _NodeData.Select(pair => (pair.Key, pair.Value));
        public IEnumerable<EdgeId> Edges => Runtime.Edges;

        protected readonly Dictionary<NodeId, NodeData> _NodeData = new Dictionary<NodeId, NodeData>();
        protected readonly Dictionary<PortId, PortData> _PortData = new Dictionary<PortId, PortData>();

        protected void AddNode(in NodeId nodeId, TNode node)
        {
            var ports = FindNodePorts(node).ToArray();
            foreach (var port in ports) _PortData[new PortId(nodeId, port.Name)] = port;
            Runtime.AddNode(nodeId, node);
            _NodeData[nodeId] = ToNodeData(nodeId, node);
        }

        public virtual void DeleteNode(in NodeId nodeId)
        {
            _NodeData.Remove(nodeId);
            var id = nodeId;
            _PortData.RemoveWhere(port => port.Key.NodeId == id);
            Runtime.DeleteNode(nodeId);
        }

        public virtual void SetNodePosition(in NodeId nodeId, float x, float y) {}

        public virtual bool IsCompatible(in PortId input, in PortId output)
        {
            var inputPort = _PortData[input];
            var outputPort = _PortData[output];
            return inputPort.Direction != outputPort.Direction &&
                   inputPort.Orientation == outputPort.Orientation &&
                   FindConnections(input).Count() < inputPort.Capacity &&
                   FindConnections(output).Count() < outputPort.Capacity &&
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

        [NotNull] protected abstract IEnumerable<PortData> FindNodePorts([NotNull] TNode node);
        protected abstract NodeData ToNodeData(in NodeId nodeId, [NotNull] TNode node);
    }

    public interface INodeProperty
    {
        int Order { get; }
    }

    public interface INodePropertyFactory
    {
        INodeProperty Create(
            object nodeObj,
            NodeId nodeId,
            UnityEditor.SerializedProperty nodeSerializedProperty = null
        );
    }

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
        public readonly int Capacity;
        public Port.Capacity PortCapacity => Capacity > 1 ? Port.Capacity.Multi : Port.Capacity.Single;
        public readonly Type PortType;

        public PortData(string name, Orientation orientation, Direction direction, int capacity, Type portType)
        {
            Name = name;
            Orientation = orientation;
            Direction = direction;
            Capacity = capacity;
            PortType = portType;
        }
    }
}