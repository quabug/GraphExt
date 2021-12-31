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
        [SerializeReference] private TNode _node;
        public TNode Node { get => _node; set => _node = value; }
        public string NodeSerializedPropertyName => nameof(_node);

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [field: SerializeField, HideInInspector] public Vector2 Position { get; set; }

        public PortId InputPort => new PortId(Id, Node.InputPortName);
        public PortId OutputPort => new PortId(Id, Node.OutputPortName);

        private Transform _parent = null;

        public event INodeComponent<TNode, TComponent>.NodeComponentDestroy OnNodeComponentDestroy;

        private bool _isDestroying = false;

        public void DestroyGameObject()
        {
            if (!_isDestroying)
            {
                _isDestroying = true;
#if UNITY_EDITOR
                GameObject.DestroyImmediate(gameObject);
#else
                GameObject.Destroy(gameObject);
#endif
            }
        }

        private void OnDestroy()
        {
            if (!_isDestroying)
            {
                _isDestroying = true;
                OnNodeComponentDestroy?.Invoke(Id);
            }
        }

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<TNode> graph)
        {
            var parentNode = transform.parent.GetComponent<TreeNodeComponent<TNode, TComponent>>();
            var edges = new HashSet<EdgeId>();
            if (parentNode != null) edges.Add(new EdgeId(InputPort, parentNode.OutputPort));
            return edges;
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
            if (input.NodeId == Id && !_isDestroying)
            {
                _parent = graph[output.NodeId].transform;
                transform.SetParent(_parent);
            }
        }

        public void OnDisconnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            if (edge.Input.NodeId == Id && !_isDestroying)
            {
                _parent = FindStageRoot();
                transform.SetParent(_parent);
            }
        }

        private Transform FindStageRoot()
        {
            var self = transform;
            while (self.parent != null) self = self.parent;
            return self;
        }

        private void Awake()
        {
            _parent = transform.parent;
        }

        private void OnTransformParentChanged()
        {
            if (transform.parent != _parent && transform.parent != null /* destroying? */)
            {
                Debug.LogWarning("it is forbidden to set parent in hierarchy window.");
                transform.SetParent(_parent, true);
            }
        }
    }
}