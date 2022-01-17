using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public class GameObjectNodes<TNode, TComponent> : IDisposable
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        public GraphRuntime<TNode> Runtime { get; }

        private readonly GameObject _root;
        private readonly BiDictionary<NodeId, TComponent> _nodeObjectMap = new BiDictionary<NodeId, TComponent>();

        public IReadOnlyDictionary<NodeId, TComponent> NodeObjectMap => _nodeObjectMap.Forward;
        public IReadOnlyDictionary<TComponent, NodeId> ObjectNodeMap => _nodeObjectMap.Reverse;
        [NotNull] public TComponent this[in NodeId id] => _nodeObjectMap[id];
        public NodeId this[[NotNull] TComponent obj] => _nodeObjectMap.GetKey(obj);

#if UNITY_EDITOR
        private readonly Dictionary<NodeId, UnityEditor.SerializedObject> _serializedObjects = new Dictionary<NodeId, UnityEditor.SerializedObject>();
        public IReadOnlyDictionary<NodeId, UnityEditor.SerializedObject> SerializedObjects => _serializedObjects;
#endif

        public GameObjectNodes([NotNull] GameObject root)
        {
            _root = root;
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

        public bool IsPortCompatible(in PortId input, in PortId output)
        {
            return IsNodeComponentPortCompatible(input.NodeId, input, output) &&
                   IsNodeComponentPortCompatible(output.NodeId, input, output);
        }

        private void AddNode(TComponent node)
        {
            _nodeObjectMap[node.Id] = node;
#if UNITY_EDITOR
            _serializedObjects[node.Id] = new UnityEditor.SerializedObject(node);
#endif
            node.OnNodeComponentConnect += OnNodeComponentConnect;
            node.OnNodeComponentDisconnect += OnNodeComponentDisconnect;
        }

        private bool IsNodeComponentPortCompatible(in NodeId nodeId, in PortId input, in PortId output)
        {
            return _nodeObjectMap[nodeId].IsPortCompatible(this, input, output);
        }

        private void OnNodeComponentConnect(in NodeId nodeId, in EdgeId edge)
        {
            Runtime.Connect(edge.Input, edge.Output);
        }

        private void OnNodeComponentDisconnect(in NodeId nodeId, in EdgeId edge)
        {
            Runtime.Disconnect(edge.Input, edge.Output);
        }

        private void OnNodeAdded(in NodeId id, TNode node)
        {
            if (!_nodeObjectMap.ContainsKey(id))
            {
                var nodeObject = new GameObject(node.GetType().Name);
                nodeObject.transform.SetParent(_root.transform);
                var nodeComponent = nodeObject.AddComponent<TComponent>();
                nodeComponent.Id = id;
                nodeComponent.Node = node;
                AddNode(nodeComponent);
                SavePrefab();
            }
        }

        private void OnNodeWillDelete(in NodeId id, TNode node)
        {
#if UNITY_EDITOR
            _serializedObjects.Remove(id);
#endif
            if (_nodeObjectMap.TryGetValue(id, out var nodeObject))
            {
                _nodeObjectMap.Remove(id);
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

        private void OnConnected(in EdgeId edge)
        {
            var inputComponent = _nodeObjectMap[edge.Input.NodeId];
            var outputComponent = _nodeObjectMap[edge.Output.NodeId];
            if (inputComponent != null) inputComponent.OnConnected(this, edge);
            if (outputComponent != null) outputComponent.OnConnected(this, edge);
            SavePrefab();
        }

        private void OnWillDisconnect(in EdgeId edge)
        {
            var inputComponent = _nodeObjectMap[edge.Input.NodeId];
            var outputComponent = _nodeObjectMap[edge.Output.NodeId];
            if (inputComponent != null) inputComponent.OnDisconnected(this, edge);
            if (outputComponent != null) outputComponent.OnDisconnected(this, edge);
            SavePrefab();
        }

        [Conditional("UNITY_EDITOR")]
        private void SavePrefab()
        {
            Editor.Utility.SavePrefabStage();
        }
    }
}