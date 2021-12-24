using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt.Prefab
{
    public interface INode
    {
        bool IsPortCompatible(PrefabGraphBackend graph, in PortId start, in PortId end);
        void OnConnected(PrefabGraphBackend graph, in PortId start, in PortId end);
        void OnDisconnected(PrefabGraphBackend graph, in PortId start, in PortId end);
    }

    public interface INodeComponent
    {
        NodeId Id { get;}
        IEnumerable<INodeProperty> Properties { get; }
        IEnumerable<(PortId id, PortData data)> Ports { get; }
        IEnumerable<EdgeId> Connections { get; }
    }

    [DisallowMultipleComponent]
    public class NodeComponent : MonoBehaviour, INodeComponent, ISerializationCallbackReceiver
    {
        [SerializeReference] public INode Node;
        [SerializeField] public Vector2 Position;
        [SerializeField] private string _nodeId = Guid.NewGuid().ToString();
        [SerializeField] private List<Connection> _serializedConnections;

        private enum NodeNameType { Hidden, GameObjectName, NodeTitleAttribute, CustomName }
        [SerializeField] private NodeNameType _nameType = NodeNameType.GameObjectName;
        [SerializeField] private string _customName;

        public NodeId Id => Guid.Parse(_nodeId);
        public GameObject GameObject => gameObject;

        private readonly IDictionary<PortId, PortData> _ports = new Dictionary<PortId, PortData>();
        public IEnumerable<(PortId id, PortData data)> Ports => _ports.Select(pair => (pair.Key, pair.Value));

        private readonly ISet<EdgeId> _connections = new HashSet<EdgeId>();
        public IEnumerable<EdgeId> Connections => _connections;

        public IEnumerable<INodeProperty> Properties => new NodePositionProperty(() => Position, position => Position = position).Yield()
            .Append<INodeProperty>(new DynamicTitleProperty(GetNodeName))
            .Concat(NodePropertyAttribute.CreateProperties(Node, Id))
            .ToArray()
        ;

        [Serializable]
        private struct Connection
        {
            public string NodeId1;
            public string PortName1;
            public string NodeId2;
            public string PortName2;
            public EdgeId ToEdge() => new EdgeId(new PortId(Guid.Parse(NodeId1), PortName1), new PortId(Guid.Parse(NodeId2), PortName2));
        }

        public void OnBeforeSerialize() {}

        public void OnAfterDeserialize()
        {
            _connections.Clear();
            foreach (var edge in _serializedConnections.Select(connection => connection.ToEdge())) _connections.Add(edge);

            _ports.Clear();
            foreach (var port in NodePortUtility.FindPorts(Node.GetType()).Select(port => ToPortPair(port))) _ports.Add(port);

            KeyValuePair<PortId, PortData> ToPortPair(in PortData port) => new KeyValuePair<PortId, PortData>(new PortId(Id, port.Name), port);
        }

        private string GetNodeName()
        {
            return _nameType switch
            {
                NodeNameType.Hidden => null,
                NodeNameType.GameObjectName => name,
                NodeNameType.NodeTitleAttribute => NodeTitleAttribute.GetTitle(Node),
                NodeNameType.CustomName => _customName,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool IsPortCompatible(PrefabGraphBackend graph, in PortId start, in PortId end)
        {
            return Node.IsPortCompatible(graph, start, end);
        }

        public void OnConnected(PrefabGraphBackend graph, in PortId start, in PortId end)
        {
            Node.OnConnected(graph, start, end);
        }

        public void OnDisconnected(PrefabGraphBackend graph, in PortId start, in PortId end)
        {
            Node.OnDisconnected(graph, start, end);
        }
    }
}