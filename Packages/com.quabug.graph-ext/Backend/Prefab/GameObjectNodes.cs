using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public class GameObjectNodes<TNode, TComponent> : ISerializableGraphBackend<TNode, TComponent>, IDisposable
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        public GraphRuntime<TNode> Runtime { get; }

        public GameObject Root { get; }
        protected readonly BiDictionary<NodeId, TComponent> _NodeObjectMap = new BiDictionary<NodeId, TComponent>();

        [NotNull] public TComponent this[in NodeId id] => _NodeObjectMap[id];
        public NodeId this[[NotNull] TComponent obj] => _NodeObjectMap.GetKey(obj);

        public IEnumerable<TComponent> Nodes => _NodeObjectMap.Values;
        public IReadOnlyBiDictionary<NodeId, TComponent> NodeMap => _NodeObjectMap;

#if UNITY_EDITOR
        private readonly Dictionary<NodeId, UnityEditor.SerializedObject> _serializedObjects = new Dictionary<NodeId, UnityEditor.SerializedObject>();
        public IReadOnlyDictionary<NodeId, UnityEditor.SerializedObject> SerializedObjects => _serializedObjects;
#endif

        public GameObjectNodes([NotNull] GameObject root)
        {
            Root = root;
            Runtime = new GraphRuntime<TNode>();
            var nodes = root.GetComponentsInChildren<TComponent>();
            foreach (var node in nodes)
            {
                AddNode(node);
                Runtime.AddNode(node.Id, node.Node);
            }
            
            foreach (var (input, output) in nodes.SelectMany(node => node.GetEdges(Runtime))) Runtime.Connect(input, output);

            Runtime.OnNodeAdded += OnNodeAdded;
            Runtime.OnNodeWillDelete += OnNodeWillDelete;
            Runtime.OnEdgeConnected += OnConnected;
            Runtime.OnEdgeWillDisconnect += OnWillDisconnect;
        }

        public void Dispose()
        {
            Runtime.OnNodeAdded -= OnNodeAdded;
            Runtime.OnNodeWillDelete -= OnNodeWillDelete;
            Runtime.OnEdgeConnected -= OnConnected;
            Runtime.OnEdgeWillDisconnect -= OnWillDisconnect;
        }

        public virtual bool IsPortCompatible(in PortId input, in PortId output)
        {
            var hasInputNode = _NodeObjectMap.TryGetValue(input.NodeId, out var inputNode);
            var hasOutputNode = _NodeObjectMap.TryGetValue(input.NodeId, out var outputNode);
            return (!hasInputNode || inputNode.IsPortCompatible(this, input: input, output: output)) &&
                   (!hasOutputNode || outputNode.IsPortCompatible(this, input: input, output: output))
            ;
        }

        public void AddNode(TComponent node)
        {
            _NodeObjectMap[node.Id] = node;
#if UNITY_EDITOR
            _serializedObjects[node.Id] = new UnityEditor.SerializedObject(node);
#endif
            node.OnNodeComponentConnect += OnNodeComponentConnect;
            node.OnNodeComponentDisconnect += OnNodeComponentDisconnect;
        }

        private void OnNodeComponentConnect(in NodeId nodeId, in EdgeId edge)
        {
            Runtime.Connect(edge.Input, edge.Output);
        }

        private void OnNodeComponentDisconnect(in NodeId nodeId, in EdgeId edge)
        {
            Runtime.Disconnect(edge.Input, edge.Output);
        }

        protected virtual void OnNodeAdded(in NodeId id, TNode node)
        {
            if (!_NodeObjectMap.ContainsKey(id))
            {
                var nodeObject = new GameObject(node.GetType().Name);
                nodeObject.transform.SetParent(Root.transform);
                var nodeComponent = nodeObject.AddComponent<TComponent>();
                nodeComponent.Id = id;
                nodeComponent.Node = node;
                AddNode(nodeComponent);
                SavePrefab();
            }
        }

        protected virtual void OnNodeWillDelete(in NodeId id, TNode node)
        {
#if UNITY_EDITOR
            _serializedObjects.Remove(id);
#endif
            if (_NodeObjectMap.TryGetValue(id, out var nodeObject))
            {
                _NodeObjectMap.Remove(id);
                if (nodeObject != null)
                {
#if UNITY_EDITOR
                    GameObject.DestroyImmediate(nodeObject.gameObject);
                    SavePrefab();
#else
                    GameObject.Destroy(nodeObject.gameObject);
#endif
                }
            }
        }

        protected virtual void OnConnected(in EdgeId edge)
        {
            _NodeObjectMap.TryGetValue(edge.Input.NodeId, out var inputComponent);
            _NodeObjectMap.TryGetValue(edge.Output.NodeId, out var outputComponent);
            if (inputComponent != null) inputComponent.OnConnected(this, edge);
            if (outputComponent != null) outputComponent.OnConnected(this, edge);
            SavePrefab();
        }

        protected virtual void OnWillDisconnect(in EdgeId edge)
        {
            _NodeObjectMap.TryGetValue(edge.Input.NodeId, out var inputComponent);
            _NodeObjectMap.TryGetValue(edge.Output.NodeId, out var outputComponent);
            if (inputComponent != null) inputComponent.OnDisconnected(this, edge);
            if (outputComponent != null) outputComponent.OnDisconnected(this, edge);
            SavePrefab();
        }

        protected void SavePrefab()
        {
#if UNITY_EDITOR
            Editor.Utility.SavePrefabStage();
#endif
        }
    }}