using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GraphExt
{
    public class GameObjectNodes<TNode> : IDisposable where TNode : INode<GraphRuntime<TNode>>
    {
        public GraphRuntime<TNode> Graph { get; }

        private readonly GameObject _root;
        private readonly BiDictionary<NodeId, GameObject> _nodeObjectMap = new BiDictionary<NodeId, GameObject>();
        private readonly Type _nodeComponentType;

        public GameObject this[in NodeId id] => _nodeObjectMap[id];

        public GameObjectNodes(GameObject root, Type nodeComponentType)
        {
            Assert.IsTrue(typeof(INodeComponent<TNode>).IsAssignableFrom(nodeComponentType));
            Assert.IsTrue(typeof(MonoBehaviour).IsAssignableFrom(nodeComponentType));

            _root = root;
            _nodeComponentType = nodeComponentType;

            Graph = new GraphRuntime<TNode>(IsPortCompatible);

            foreach (var node in root.GetComponentsInChildren<INodeComponent<TNode>>())
                AddNodeDataToGraphRuntime(node);

            Graph.OnNodeAdded += OnNodeAdded;
            Graph.OnNodeAdded += OnNodeDeleted;
            Graph.OnEdgeConnected += OnConnected;
            Graph.OnEdgeDisconnected += OnDisconnected;

            void AddNodeDataToGraphRuntime(INodeComponent<TNode> node)
            {
                Graph.AddNode(node.Id, node.Node);
                foreach (var edge in node.Edges) Graph.Connect(edge.Input, edge.Output);
            }
        }

        public void Dispose()
        {
            Graph.OnNodeAdded -= OnNodeAdded;
            Graph.OnNodeAdded -= OnNodeDeleted;
            Graph.OnEdgeConnected -= OnConnected;
            Graph.OnEdgeDisconnected -= OnDisconnected;
        }

        public void SetPosition(in NodeId id, Vector2 position)
        {
            _nodeObjectMap[id].GetComponent<INodeComponent<TNode>>().Position = position;
        }

        private bool IsPortCompatible(in PortId input, in PortId output)
        {
            return IsNodeComponentPortCompatible(input.NodeId, input, output) &&
                   IsNodeComponentPortCompatible(output.NodeId, input, output);
        }

        private bool IsNodeComponentPortCompatible(in NodeId nodeId, in PortId input, in PortId output)
        {
            return _nodeObjectMap[nodeId].GetComponent<INodeComponent<TNode>>().IsPortCompatible(this, input, output);
        }

        private void OnNodeAdded(in NodeId id, TNode node)
        {
            var nodeObject = new GameObject(node.GetType().Name);
            nodeObject.transform.SetParent(_root.transform);
            var nodeComponent = (INodeComponent<TNode>)nodeObject.AddComponent(_nodeComponentType);
            nodeComponent.Id = id;
            nodeComponent.Node = node;
            _nodeObjectMap[id] = nodeObject;
        }

        private void OnNodeDeleted(in NodeId id, TNode node)
        {
            if (_nodeObjectMap.TryGetValue(id, out var nodeObject))
            {
                _nodeObjectMap.Remove(id);
#if UNITY_EDITOR
                GameObject.DestroyImmediate(nodeObject);
#else
                GameObject.Destroy(nodeObject);
#endif
            }
        }

        private void OnConnected(in EdgeId edge)
        {

        }

        private void OnDisconnected(in EdgeId edge)
        {

        }
    }

    public interface INodeComponent<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        NodeId Id { get; set; }
        TNode Node { get; set; }
        Vector2 Position { get; set; }
        IReadOnlySet<EdgeId> Edges { get; }

        bool IsPortCompatible(GameObjectNodes<TNode> data, in PortId input, in PortId output);
        void OnConnected(GameObjectNodes<TNode> data, in PortId input, in PortId output);
        void OnDisconnected(GameObjectNodes<TNode> data, in PortId input, in PortId output);
    }

    [DisallowMultipleComponent]
    public abstract class NodeComponent<TNode> : MonoBehaviour, INodeComponent<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        [SerializeReference] private TNode _node;
        public TNode Node
        {
            get => _node;
            set => _node = value;
        }

        [SerializeField, HideInInspector] private string _nodeId = Guid.NewGuid().ToString();
        public NodeId Id
        {
            get => Guid.Parse(_nodeId);
            set => _nodeId = value.Id.ToString();
        }

        [field: SerializeField, HideInInspector] public Vector2 Position { get; set; }

        private enum NodeNameType { Hidden, GameObjectName, NodeTitleAttribute, CustomName }
        [SerializeField] private NodeNameType _nameType = NodeNameType.NodeTitleAttribute;
        [SerializeField] private string _customName;

        bool INodeComponent<TNode>.IsPortCompatible(GameObjectNodes<TNode> data, in PortId input, in PortId output)
        {
            return IsPortCompatible(data, input, output);
        }

        void INodeComponent<TNode>.OnConnected(GameObjectNodes<TNode> data, in PortId input, in PortId output)
        {
            OnConnected(data, input, output);
        }

        void INodeComponent<TNode>.OnDisconnected(GameObjectNodes<TNode> data, in PortId input, in PortId output)
        {
            OnDisconnected(data, input, output);
        }

        public abstract IReadOnlySet<EdgeId> Edges { get; }
        protected virtual bool IsPortCompatible(GameObjectNodes<TNode> graph, in PortId input, in PortId output) => true;
        protected virtual void OnConnected(GameObjectNodes<TNode> graph, in PortId input, in PortId output) {}
        protected virtual void OnDisconnected(GameObjectNodes<TNode> graph, in PortId input, in PortId output) {}
    }
}