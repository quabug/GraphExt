using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt.Prefab
{
    public interface INode
    {
        NodeId Id { get; set; }
        bool IsPortCompatible(PrefabGraphBackend graph, in PortId input, in PortId output);
        void OnConnected(PrefabGraphBackend graph, in PortId input, in PortId output);
        void OnDisconnected(PrefabGraphBackend graph, in PortId input, in PortId output);
    }

    public interface INodeComponent
    {
        NodeId Id { get;}
        INode Node { get; }
        IEnumerable<INodeProperty> Properties { get; }
        IEnumerable<(PortId id, PortData data)> Ports { get; }
        IEnumerable<EdgeId> Connections { get; }

        bool IsPortCompatible(PrefabGraphBackend graph, in PortId input, in PortId output);
        void OnConnected(PrefabGraphBackend graph, in PortId input, in PortId output);
        void OnDisconnected(PrefabGraphBackend graph, in PortId input, in PortId output);
    }

    [DisallowMultipleComponent]
    public abstract class NodeComponent : MonoBehaviour, INodeComponent
    {
        [SerializeReference] private INode _node;
        public INode Node { get => _node; set => _node = value; }
        [SerializeField, HideInInspector] public Vector2 Position;
        [SerializeField, HideInInspector] private string _nodeId = Guid.NewGuid().ToString();

        private enum NodeNameType { Hidden, GameObjectName, NodeTitleAttribute, CustomName }
        [SerializeField] private NodeNameType _nameType = NodeNameType.NodeTitleAttribute;
        [SerializeField] private string _customName;

        public NodeId Id => Guid.Parse(_nodeId);

        private readonly Lazy<IDictionary<PortId, PortData>> _ports;
        public IEnumerable<(PortId id, PortData data)> Ports => _ports.Value.Select(pair => (pair.Key, pair.Value));

        public IEnumerable<INodeProperty> Properties => CreatePositionProperty()
            .Append(new DynamicTitleProperty(GetNodeName))
            .Concat(CreateNodeProperties())
            .ToArray()
        ;

        private IEnumerable<INodeProperty> CreateNodeProperties()
        {
#if UNITY_EDITOR
            var serializedObject = new UnityEditor.SerializedObject(this);
            var nodeSerializedProperty = serializedObject.FindProperty(nameof(_node));
            return NodePropertyAttribute.CreateProperties(Node, Id, nodeSerializedProperty);
#else
            return NodePropertyAttribute.CreateProperties(Node, Id);
#endif
        }

        private IEnumerable<INodeProperty> CreatePositionProperty()
        {
            yield return new NodePositionProperty(() => Position, position =>
            {
                Position = position;
                gameObject.scene.SaveScene();
            });
        }

        public abstract IEnumerable<EdgeId> Connections { get; }

        public NodeComponent()
        {
            _ports = new Lazy<IDictionary<PortId, PortData>>(() =>
                NodePortUtility.FindPorts(Node.GetType()).ToDictionary(port => new PortId(Id, port.Name), port => port)
            );
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

        bool INodeComponent.IsPortCompatible(PrefabGraphBackend graph, in PortId input, in PortId output)
        {
            return IsPortCompatible(graph, input, output) && Node.IsPortCompatible(graph, input, output);
        }

        void INodeComponent.OnConnected(PrefabGraphBackend graph, in PortId input, in PortId output)
        {
            OnConnected(graph, input, output);
            Node.OnConnected(graph, input, output);
        }

        void INodeComponent.OnDisconnected(PrefabGraphBackend graph, in PortId input, in PortId output)
        {
            OnDisconnected(graph, input, output);
            Node.OnDisconnected(graph, input, output);
        }

        protected virtual bool IsPortCompatible(PrefabGraphBackend graph, in PortId input, in PortId output) => true;
        protected virtual void OnConnected(PrefabGraphBackend graph, in PortId input, in PortId output) {}
        protected virtual void OnDisconnected(PrefabGraphBackend graph, in PortId input, in PortId output) {}

    }
}