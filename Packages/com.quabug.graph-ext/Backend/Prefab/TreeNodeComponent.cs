using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt
{
    public interface ITreeNode<in TGraph> : INode<TGraph>
    {
        string InputPortName { get; }
        string OutputPortName { get; }
    }

    [ExecuteAlways]
    [AddComponentMenu("")]
    public class TreeNodeComponent<TNode, TComponent> : MonoBehaviour, INodeComponent<TNode, TComponent>
        where TNode : ITreeNode<GraphRuntime<TNode>>
        where TComponent : TreeNodeComponent<TNode, TComponent>
    {
        [SerializeReference] protected TNode _node;
        public TNode Node { get => _node; set => _node = value; }
        public string NodeSerializedPropertyName => nameof(_node);

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [field: SerializeField, HideInInspector] public Vector2 Position { get; set; }

        public PortId InputPort => new PortId(Id, Node.InputPortName);
        public PortId OutputPort => new PortId(Id, Node.OutputPortName);

        public event INodeComponent<TNode, TComponent>.NodeComponentConnect OnNodeComponentConnect;
        public event INodeComponent<TNode, TComponent>.NodeComponentDisconnect OnNodeComponentDisconnect;

        private bool _isTransforming = false;

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<TNode> graph)
        {
            var edge = GetEdge();
            var edges = new HashSet<EdgeId>();
            if (edge.HasValue) edges.Add(edge.Value);
            return edges;
        }

        private EdgeId? GetEdge()
        {
            if (transform.parent == null) return null;
            var parentNode = transform.parent.GetComponent<TreeNodeComponent<TNode, TComponent>>();
            if (parentNode == null) return null;
            return new EdgeId(InputPort, parentNode.OutputPort);
        }

        public bool IsPortCompatible(GameObjectNodes<TNode, TComponent> graph, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            if (input.NodeId == Id) return true; // only check compatible on output/start node
            // cannot connect to input/end node which is parent of output/start node
            var inputNodeId = input.NodeId;
            return GetComponentsInParent<TreeNodeComponent<TNode, TComponent>>().All(node => node.Id != inputNodeId);
        }

        public void OnConnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            var (input, output) = edge;
            if (input.NodeId == Id && !_isTransforming)
            {
                _isTransforming = true;
                transform.SetParent(graph[output.NodeId].transform);
                _isTransforming = false;
            }
        }

        public void OnDisconnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            if (edge.Input.NodeId == Id && !_isTransforming)
            {
                _isTransforming = true;
                transform.SetParent(FindStageRoot());
                _isTransforming = false;
            }
        }

        private Transform FindStageRoot()
        {
            var self = transform;
            while (self.parent != null) self = self.parent;
            return self;
        }

        private void OnBeforeTransformParentChanged()
        {
            if (_isTransforming) return;

            var edge = GetEdge();
            if (edge.HasValue)
            {
                _isTransforming = true;
                OnNodeComponentDisconnect?.Invoke(Id, edge.Value);
                _isTransforming = false;
            }
        }

        private void OnTransformParentChanged()
        {
            if (_isTransforming) return;

            var edge = GetEdge();
            if (edge.HasValue)
            {
                _isTransforming = true;
                OnNodeComponentConnect?.Invoke(Id, edge.Value);
                _isTransforming = false;
            }
        }
    }
}