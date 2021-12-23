using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Memory
{
    public class MemoryGraphBackend : BaseGraphBackend
    {
        public class Node
        {
            [NotNull] public IMemoryNode Inner { get; }
            public Vector2 Position { get; set; }
            public NodeId Id { get; }
            public Node(IMemoryNode node, NodeId id, Vector2 position)
            {
                Inner = node;
                Id = id;
                Position = position;
            }
        }

        protected readonly Dictionary<NodeId, Node> _MemoryNodeMap = new Dictionary<NodeId, Node>();
        public IReadOnlyDictionary<NodeId, Node> MemoryNodeMap => _MemoryNodeMap;

        protected readonly Dictionary<IMemoryNode, NodeId> _MemoryNodeIdMap = new Dictionary<IMemoryNode, NodeId>();
        public NodeId this[IMemoryNode node] => _MemoryNodeIdMap[node];

        public MemoryGraphBackend() {}

        public MemoryGraphBackend([NotNull] IReadOnlyList<Node> nodes, [NotNull] IReadOnlyList<EdgeId> edges)
            : base(nodes.Select(ToNodeData), nodes.SelectMany(FindPorts), edges)
        {
            _MemoryNodeMap = nodes.ToDictionary(node => node.Id, node => node);
            _MemoryNodeIdMap = nodes.ToDictionary(node => node.Inner, node => node.Id);
        }

        private static IEnumerable<(PortId, PortData)> FindPorts(Node node)
        {
            return NodePortUtility.FindPorts(node.Inner.GetType()).Select(port => (new PortId(node.Id, port.Name), port));
        }

        private static (NodeId id, NodeData data) ToNodeData([NotNull] Node node)
        {
            return (node.Id, new NodeData(CreatePositionProperty(node).Yield()
                    .Concat(NodeTitleAttribute.CreateTitleProperty(node.Inner))
                    .Concat(NodePropertyAttribute.CreateProperties(node.Inner, node.Id))
                    .ToArray()
            ));

            INodeProperty CreatePositionProperty(Node node)
            {
                return new NodePositionProperty(() => node.Position, position => node.Position = position);
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
            var node = new Node(innerNode, Guid.NewGuid(), Vector2.zero);
            _MemoryNodeMap[node.Id] = node;
            _MemoryNodeIdMap[innerNode] = node.Id;
            _NodeMap.Add(node.Id, ToNodeData(node).data);
            foreach (var (portId, portData) in FindPorts(node)) _PortMap.Add(portId, portData);
            return node;
        }

        [NotNull] public ISet<PortId> FindConnectedPorts(IMemoryNode node, string port)
        {
            return FindConnectedPorts(new PortId(this[node], port));
        }
    }
}