using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public class GraphScriptableObject<TNode, TNodeScriptableObject> : ScriptableObject
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        public GraphRuntime<TNode> Runtime { get; private set; } = new GraphRuntime<TNode>();
        private BiDictionary<NodeId, TNodeScriptableObject> _nodeObjectMap = new BiDictionary<NodeId, TNodeScriptableObject>();
        [NotNull] public TNodeScriptableObject this[in NodeId nodeId] => _nodeObjectMap[nodeId];

        [SerializeField, HideInInspector] private List<TNodeScriptableObject> _nodes = new List<TNodeScriptableObject>();

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            UnInitialize();
            Runtime = new GraphRuntime<TNode>();
            _nodeObjectMap = new BiDictionary<NodeId, TNodeScriptableObject>();
            foreach (var node in _nodes)
            {
                Runtime.AddNode(node.Id, node.Node);
                _nodeObjectMap[node.Id] = node;
            }

            foreach (var (input, output) in _nodes.SelectMany(node => node.Edges).Where(edge => !Runtime.Edges.Contains(edge)))
            {
                Runtime.Connect(input, output);
            }

            Runtime.OnNodeAdded += OnNodeAdded;
            Runtime.OnNodeWillDelete += OnNodeWillDelete;
            Runtime.OnEdgeConnected += OnConnected;
            Runtime.OnEdgeWillDisconnect += OnWillDisconnect;
        }

        public void UnInitialize()
        {
            Runtime.OnNodeAdded -= OnNodeAdded;
            Runtime.OnNodeWillDelete -= OnNodeWillDelete;
            Runtime.OnEdgeConnected -= OnConnected;
            Runtime.OnEdgeWillDisconnect -= OnWillDisconnect;
        }

        private void OnDestroy()
        {
            UnInitialize();
        }

        public void SetPosition(in NodeId id, Vector2 position)
        {
            _nodeObjectMap[id].Position = position;
        }

        private void OnNodeAdded(in NodeId id, TNode node)
        {
            var nodeInstance = CreateInstance<TNodeScriptableObject>();
            nodeInstance.hideFlags = HideFlags.None;
            nodeInstance.Id = id;
            nodeInstance.Node = node;
            _nodes.Add(nodeInstance);
            _nodeObjectMap[id] = nodeInstance;
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.AddObjectToAsset(nodeInstance, this);
#endif
        }

        private void OnNodeWillDelete(in NodeId id, TNode node)
        {
            if (_nodeObjectMap.TryGetValue(id, out var nodeObject))
            {
                _nodeObjectMap.Remove(id);
                _nodes.Remove(nodeObject);
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.RemoveObjectFromAsset(nodeObject);
#endif
            }
        }

        private void OnConnected(in EdgeId edge)
        {
            var inputComponent = _nodeObjectMap[edge.Input.NodeId];
            var outputComponent = _nodeObjectMap[edge.Output.NodeId];
            inputComponent.OnConnected(edge);
            outputComponent.OnConnected(edge);
        }

        private void OnWillDisconnect(in EdgeId edge)
        {
            var inputComponent = _nodeObjectMap[edge.Input.NodeId];
            var outputComponent = _nodeObjectMap[edge.Output.NodeId];
            inputComponent.OnDisconnected(edge);
            outputComponent.OnDisconnected(edge);
        }
    }
}