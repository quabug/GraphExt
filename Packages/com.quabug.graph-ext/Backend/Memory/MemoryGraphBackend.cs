using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Memory
{
    [Serializable]
    public class MemoryGraphBackend : BaseGraphBackend
    {
        public class Node
        {
            [NotNull] public IMemoryNode Inner { get; }
            public float PositionX = 0;
            public float PositionY = 0;
            public Guid Id { get; private set; }
            public Node([NotNull] IMemoryNode inner, Guid nodeId) => (Inner, Id) = (inner, nodeId);

            public Type GetNodeType() => Inner.GetType();
            public NodeId GetNodeId() => Id;
            public void SetPosition(Vector2 pos) => (PositionX, PositionY) = (pos.x, pos.y);
            public Vector2 GetPosition() => new Vector2(PositionX, PositionY);
        }

        protected Dictionary<NodeId, Node> _MemoryNodeMap = new Dictionary<NodeId, Node>();
        public IReadOnlyDictionary<NodeId, Node> MemoryNodeMap => _MemoryNodeMap;

        protected Dictionary<IMemoryNode, NodeId> _MemoryNodeIdMap = new Dictionary<IMemoryNode, NodeId>();
        public NodeId this[IMemoryNode node] => _MemoryNodeIdMap[node];

        public MemoryGraphBackend() {}

        public MemoryGraphBackend([NotNull] IReadOnlyList<Node> nodes, [NotNull] IReadOnlyList<EdgeId> edges)
            : base(nodes.Select(ToNodeData), nodes.SelectMany(FindPorts), edges)
        {
            _MemoryNodeMap = nodes.ToDictionary(node => node.GetNodeId(), node => node);
            _MemoryNodeIdMap = nodes.ToDictionary(node => node.Inner, node => node.GetNodeId());
        }

        private static IEnumerable<(PortId, PortData)> FindPorts(Node node)
        {
            return NodePortUtility.FindPorts(node.GetNodeType()).Select(port => (new PortId(node.GetNodeId(), port.Name), port));
        }

        private static (NodeId id, NodeData data) ToNodeData([NotNull] Node node)
        {
            return (node.GetNodeId(), new NodeData(CreatePositionProperty(node).Yield()
                    .Concat(NodeTitleAttribute.CreateTitleProperty(node.Inner))
                    .Concat(NodePropertyAttribute.CreateProperties(node.Inner, node.GetNodeId()))
                    .ToArray()
            ));

            INodeProperty CreatePositionProperty(Node node)
            {
                return new NodePositionProperty(node.GetPosition, node.SetPosition);
            }
        }

        public override bool IsCompatible(in PortId input, in PortId output)
        {
            var inputData = _PortMap[input];
            var outputData = _PortMap[output];
            return inputData.Direction != outputData.Direction &&
                   inputData.PortType == outputData.PortType &&
                   GetMemoryNodeByPort(input).IsPortCompatible(this, output, input) &&
                   GetMemoryNodeByPort(output).IsPortCompatible(this, output, input)
            ;
        }

        public IMemoryNode GetMemoryNodeByPort(in PortId port) => GetMemoryNode(port.NodeId);
        public IMemoryNode GetMemoryNode(in NodeId node) => _MemoryNodeMap[node].Inner;

        protected override void OnConnected(in PortId input, in PortId output)
        {
            GetMemoryNodeByPort(input).OnConnected(this, output, input);
            GetMemoryNodeByPort(output).OnConnected(this, output, input);
        }

        protected override void OnDisconnected(in PortId input, in PortId output)
        {
            GetMemoryNodeByPort(input).OnDisconnected(this, output, input);
            GetMemoryNodeByPort(output).OnDisconnected(this, output, input);
        }

        protected override void OnNodeDeleted(in NodeId nodeId)
        {
            var node = _MemoryNodeMap[nodeId];
            _MemoryNodeMap.Remove(nodeId);
            _MemoryNodeIdMap.Remove(node.Inner);
        }

        public Node CreateNode(IMemoryNode innerNode)
        {
            var node = new Node(innerNode, Guid.NewGuid());
            _MemoryNodeMap[node.GetNodeId()] = node;
            _MemoryNodeIdMap[innerNode] = node.GetNodeId();
            _NodeMap.Add(node.GetNodeId(), ToNodeData(node).data);
            foreach (var (portId, portData) in FindPorts(node)) _PortMap.Add(portId, portData);
            return node;
        }

        [NotNull] public ISet<PortId> FindConnectedPorts(IMemoryNode node, string port)
        {
            return FindConnectedPorts(new PortId(this[node], port));
        }
    }
}