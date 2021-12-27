﻿using System;
using System.Collections.Generic;
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
            Graph = new GraphRuntime<TNode>(IsPortCompatible);
            foreach (var node in root.GetComponentsInChildren<TComponent>()) AddNodeDataToGraphRuntime(node);
            Graph.OnNodeAdded += OnNodeAdded;
            Graph.OnNodeAdded += OnNodeDeleted;
            Graph.OnEdgeConnected += OnConnected;
            Graph.OnEdgeDisconnected += OnDisconnected;

            void AddNodeDataToGraphRuntime(TComponent node)
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
            _nodeObjectMap[id].GetComponent<TComponent>().Position = position;
        }

        private bool IsPortCompatible(in PortId input, in PortId output)
        {
            return IsNodeComponentPortCompatible(input.NodeId, input, output) &&
                   IsNodeComponentPortCompatible(output.NodeId, input, output);
        }

        private bool IsNodeComponentPortCompatible(in NodeId nodeId, in PortId input, in PortId output)
        {
            return _nodeObjectMap[nodeId].GetComponent<TComponent>().IsPortCompatible(this, input, output);
        }

        private void OnNodeAdded(in NodeId id, TNode node)
        {
            var nodeObject = new GameObject(node.GetType().Name);
            nodeObject.transform.SetParent(_root.transform);
            var nodeComponent = nodeObject.AddComponent<TComponent>();
            nodeComponent.Id = id;
            nodeComponent.Node = node;
            _nodeObjectMap[id] = nodeComponent;
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
            var inputComponent = _nodeObjectMap[edge.Input.NodeId].GetComponent<TComponent>();
            var outputComponent = _nodeObjectMap[edge.Output.NodeId].GetComponent<TComponent>();
            inputComponent.OnConnected(this, edge);
            outputComponent.OnConnected(this, edge);
        }

        private void OnDisconnected(in EdgeId edge)
        {
            var inputComponent = _nodeObjectMap[edge.Input.NodeId].GetComponent<TComponent>();
            var outputComponent = _nodeObjectMap[edge.Output.NodeId].GetComponent<TComponent>();
            inputComponent.OnDisconnected(this, edge);
            outputComponent.OnDisconnected(this, edge);
        }
    }

    public interface INodeComponent<TNode, TComponent>
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        NodeId Id { get; set; }
        TNode Node { get; set; }
        string NodeSerializedPropertyName { get; }
        Vector2 Position { get; set; }
        IReadOnlySet<EdgeId> Edges { get; }

        bool IsPortCompatible(GameObjectNodes<TNode, TComponent> data, in PortId input, in PortId output);
        void OnConnected(GameObjectNodes<TNode, TComponent> data, in EdgeId edge);
        void OnDisconnected(GameObjectNodes<TNode, TComponent> data, in EdgeId edge);
    }
}