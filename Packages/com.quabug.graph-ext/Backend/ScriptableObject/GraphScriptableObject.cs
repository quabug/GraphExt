using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public class GraphScriptableObject<TNode, TNodeScriptableObject> : ScriptableObject, ISerializableGraphBackend<TNode, TNodeScriptableObject>
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        public GraphRuntime<TNode> Runtime { get; private set; } = new GraphRuntime<TNode>();
        private BiDictionary<NodeId, TNodeScriptableObject> _nodesCache = new BiDictionary<NodeId, TNodeScriptableObject>();
        [NotNull] public TNodeScriptableObject this[in NodeId nodeId] => _nodesCache[nodeId];
        public NodeId this[[NotNull] TNodeScriptableObject node] => _nodesCache.GetKey(node);

        [SerializeField] private List<TNodeScriptableObject> _nodes = new List<TNodeScriptableObject>();

        public IReadOnlyList<TNodeScriptableObject> Nodes => _nodes;
        public IReadOnlyDictionary<NodeId, TNodeScriptableObject> NodeObjectMap => _nodesCache.Forward;
        public IReadOnlyDictionary<TNodeScriptableObject, NodeId> ObjectNodeMap => _nodesCache.Reverse;

#if UNITY_EDITOR
        private Dictionary<NodeId, UnityEditor.SerializedObject> _serializedObjects;
        public IReadOnlyDictionary<NodeId, UnityEditor.SerializedObject> SerializedObjects => _serializedObjects;
#endif

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            UnInitialize();
            Runtime = new GraphRuntime<TNode>();
            _nodesCache = new BiDictionary<NodeId, TNodeScriptableObject>();
#if UNITY_EDITOR
            _serializedObjects = new Dictionary<NodeId, UnityEditor.SerializedObject>();
#endif
            for (var i = _nodes.Count - 1; i >= 0; i--)
            {
                var node = _nodes[i];
                if (node.Node == null)
                {
                    // TODO: log
                    _nodes.RemoveAt(i);
                }
                else
                {
#if UNITY_EDITOR
                    _serializedObjects.Add(node.Id, new UnityEditor.SerializedObject(node));
#endif
                    Runtime.AddNode(node.Id, node.Node);
                    _nodesCache[node.Id] = node;
                }
            }

            foreach (var (input, output) in _nodes
                         .SelectMany(node => node.GetEdges(Runtime))
                         .Where(edge => !Runtime.Edges.Contains(edge)))
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

        private void OnNodeAdded(in NodeId id, TNode node)
        {
            var nodeInstance = CreateInstance<TNodeScriptableObject>();
            nodeInstance.hideFlags = HideFlags.None;
            nodeInstance.Id = id;
            nodeInstance.Node = node;
            _nodes.Add(nodeInstance);
            _nodesCache[id] = nodeInstance;
#if UNITY_EDITOR
            _serializedObjects.Add(id, new UnityEditor.SerializedObject(nodeInstance));
            UnityEditor.AssetDatabase.AddObjectToAsset(nodeInstance, this);
            nodeInstance.name = node.GetType().Name;
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        private void OnNodeWillDelete(in NodeId id, TNode node)
        {
            if (_nodesCache.TryGetValue(id, out var nodeObject))
            {
                _nodesCache.Remove(id);
                _nodes.Remove(nodeObject);
#if UNITY_EDITOR
                _serializedObjects.Remove(id);
                UnityEditor.AssetDatabase.RemoveObjectFromAsset(nodeObject);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }

        private void OnConnected(in EdgeId edge)
        {
            var inputComponent = _nodesCache[edge.Input.NodeId];
            var outputComponent = _nodesCache[edge.Output.NodeId];
            inputComponent.OnConnected(Runtime, edge);
            outputComponent.OnConnected(Runtime, edge);
        }

        private void OnWillDisconnect(in EdgeId edge)
        {
            var inputComponent = _nodesCache[edge.Input.NodeId];
            var outputComponent = _nodesCache[edge.Output.NodeId];
            inputComponent.OnDisconnected(Runtime, edge);
            outputComponent.OnDisconnected(Runtime, edge);
        }
    }
}