using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public class GameObjectNodes<TNode, TComponent> : IDisposable
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        public GraphRuntime<TNode> Graph { get; }

        private readonly GameObject _root;
        private readonly BiDictionary<NodeId, TComponent> _nodeObjectMap = new BiDictionary<NodeId, TComponent>();

        public IReadOnlyDictionary<NodeId, TComponent> NodeObjectMap => _nodeObjectMap.Forward;
        public IReadOnlyDictionary<TComponent, NodeId> ObjectNodeMap => _nodeObjectMap.Reverse;
        [NotNull] public TComponent this[in NodeId id] => _nodeObjectMap[id];
        public NodeId this[[NotNull] TComponent obj] => _nodeObjectMap.GetKey(obj);

        public GameObjectNodes()
        {
            Graph = new GraphRuntime<TNode>();
        }

        public GameObjectNodes([NotNull] GameObject root)
        {
            _root = root;
            Graph = new GraphRuntime<TNode>();
            var nodes = root.GetComponentsInChildren<TComponent>();
            foreach (var node in nodes)
            {
                AddNode(node);
                Graph.AddNode(node.Id, node.Node);
            }
            
            foreach (var (input, output) in nodes.SelectMany(node => node.GetEdges(Graph))) Graph.Connect(input, output);

            Graph.OnNodeAdded += OnNodeAdded;
            Graph.OnNodeWillDelete += OnNodeWillDelete;
            Graph.OnEdgeConnected += OnConnected;
            Graph.OnEdgeWillDisconnect += OnWillDisconnect;
        }

        public void Dispose()
        {
            Graph.OnNodeAdded -= OnNodeAdded;
            Graph.OnNodeWillDelete -= OnNodeWillDelete;
            Graph.OnEdgeConnected -= OnConnected;
            Graph.OnEdgeWillDisconnect -= OnWillDisconnect;
        }

        public void Refresh()
        {
            if (_root == null) return;

            var nodes = _root.GetComponentsInChildren<TComponent>();
            var removedNodes = new HashSet<NodeId>(Graph.NodeMap.Keys);
            var addedNodes = new HashSet<TComponent>();
            foreach (var node in nodes)
            {
                if (_nodeObjectMap.ContainsKey(node.Id)) removedNodes.Remove(node.Id);
                else addedNodes.Add(node);
            }

            foreach (var node in addedNodes)
            {
                AddNode(node);
                Graph.AddNode(node.Id, node.Node);
            }

            foreach (var (input, output) in addedNodes.SelectMany(node => node.GetEdges(Graph)))
                Graph.Connect(input, output);

            foreach (var nodeId in removedNodes) Graph.DeleteNode(nodeId);
        }

        public void SetPosition(in NodeId id, Vector2 position)
        {
            _nodeObjectMap[id].Position = position;
        }

        public bool IsPortCompatible(in PortId input, in PortId output)
        {
            return IsNodeComponentPortCompatible(input.NodeId, input, output) &&
                   IsNodeComponentPortCompatible(output.NodeId, input, output);
        }

        private void AddNode(TComponent node)
        {
            _nodeObjectMap[node.Id] = node;
            node.OnNodeComponentConnect += OnNodeComponentConnect;
            node.OnNodeComponentDisconnect += OnNodeComponentDisconnect;
        }

        private bool IsNodeComponentPortCompatible(in NodeId nodeId, in PortId input, in PortId output)
        {
            return _nodeObjectMap[nodeId].IsPortCompatible(this, input, output);
        }

        private void OnNodeComponentConnect(in NodeId nodeId, in EdgeId edge)
        {
            Graph.Connect(edge.Input, edge.Output);
        }

        private void OnNodeComponentDisconnect(in NodeId nodeId, in EdgeId edge)
        {
            Graph.Disconnect(edge.Input, edge.Output);
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
            }
        }

        private void OnNodeWillDelete(in NodeId id, TNode node)
        {
            if (_nodeObjectMap.TryGetValue(id, out var nodeObject))
            {
                _nodeObjectMap.Remove(id);
                if (nodeObject != null)
                {
    #if UNITY_EDITOR
                    GameObject.DestroyImmediate(nodeObject.gameObject);
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
        }

        private void OnWillDisconnect(in EdgeId edge)
        {
            var inputComponent = _nodeObjectMap[edge.Input.NodeId];
            var outputComponent = _nodeObjectMap[edge.Output.NodeId];
            if (inputComponent != null) inputComponent.OnDisconnected(this, edge);
            if (outputComponent != null) outputComponent.OnDisconnected(this, edge);
        }
    }

}