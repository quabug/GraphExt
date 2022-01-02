using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt
{
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

        [SerializeField, HideInInspector] private List<SerializableEdge> _serializableEdges = new List<SerializableEdge>();
        private readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        public PortId InputPort => new PortId(Id, Node.InputPortName);
        public PortId OutputPort => new PortId(Id, Node.OutputPortName);

        public event INodeComponent<TNode, TComponent>.NodeComponentConnect OnNodeComponentConnect;
        public event INodeComponent<TNode, TComponent>.NodeComponentDisconnect OnNodeComponentDisconnect;

        private bool _isTransforming = false;

        IReadOnlySet<EdgeId> INodeComponent<TNode, TComponent>.GetEdges(GraphRuntime<TNode> graph)
        {
            _edges.Clear();
            _edges.UnionWith(GetEdges(graph));
            foreach (var serializableEdge in _serializableEdges)
            {
                try
                {
                    _edges.Add(serializableEdge.ToEdge(graph));
                }
                catch
                {
                    Debug.LogWarning($"invalid edge {serializableEdge.OutputNode}.{serializableEdge.OutputPort}->{serializableEdge.InputNode}.{serializableEdge.InputPort}");
                }
            }
            var treeEdge = GetTreeEdge();
            if (treeEdge.HasValue) _edges.Add(treeEdge.Value);
            return _edges;
        }

        private EdgeId? GetTreeEdge()
        {
            if (transform.parent == null) return null;
            var parentNode = transform.parent.GetComponent<TreeNodeComponent<TNode, TComponent>>();
            if (parentNode == null) return null;
            return new EdgeId(InputPort, parentNode.OutputPort);
        }

        bool INodeComponent<TNode, TComponent>.IsPortCompatible(GameObjectNodes<TNode, TComponent> graph, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            if (input.NodeId == Id) return true; // only check compatible on output/start node

            // tree port must connect to another tree port
            if (output == OutputPort && graph[input.NodeId].InputPort != input) return false;
            if (output != OutputPort && graph[input.NodeId].InputPort == input) return false;

            // cannot connect to input/end node which is parent of output/start node to avoid circle dependency
            var inputPort = input;
            var isParentConnection = GetComponentsInParent<TreeNodeComponent<TNode, TComponent>>().Any(node => node.InputPort == inputPort);

            return !isParentConnection && IsPortCompatible(graph, input, output);
        }

        void INodeComponent<TNode, TComponent>.OnConnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
            _edges.Add(edge);
            var (input, output) = edge;
            // set parent for tree edges
            if (!_isTransforming && input == InputPort)
            {
                _isTransforming = true;
                transform.SetParent(graph[output.NodeId].transform);
                _isTransforming = false;
            }
            // save non-tree output edges
            if (input != InputPort && output != OutputPort && output.NodeId == Id)
            {
                var serializableEdge = edge.ToSerializable(graph.Graph);
                _serializableEdges.Add(serializableEdge);
            }
            OnConnected(graph, edge);
        }

        void INodeComponent<TNode, TComponent>.OnDisconnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;

            try
            {
                OnDisconnected(graph, edge);
            }
            finally
            {
                var (input, output) = edge;

                // delete non-tree output edges
                if (input != InputPort && output != OutputPort && output.NodeId == Id) _serializableEdges.Remove(edge.ToSerializable());

                // reset parent for tree edges
                if (!_isTransforming && edge.Input == InputPort)
                {
                    _isTransforming = true;
                    transform.SetParent(FindStageRoot());
                    _isTransforming = false;
                }

                _edges.Remove(edge);
            }
        }

        private Transform FindStageRoot()
        {
            var self = transform;
            while (self.parent != null) self = self.parent;
            return self;
        }

        protected virtual void OnBeforeTransformParentChanged()
        {
            if (_isTransforming) return;

            var edge = GetTreeEdge();
            if (edge.HasValue)
            {
                _isTransforming = true;
                OnNodeComponentDisconnect?.Invoke(Id, edge.Value);
                _isTransforming = false;
            }
        }

        protected virtual void OnTransformParentChanged()
        {
            if (_isTransforming) return;

            var edge = GetTreeEdge();
            if (edge.HasValue)
            {
                _isTransforming = true;
                OnNodeComponentConnect?.Invoke(Id, edge.Value);
                _isTransforming = false;
            }
        }

        protected virtual IReadOnlySet<EdgeId> GetEdges(GraphRuntime<TNode> graph) => new HashSet<EdgeId>();
        protected virtual bool IsPortCompatible(GameObjectNodes<TNode, TComponent> graph, in PortId input, in PortId output) => true;
        protected virtual void OnConnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge) {}
        protected virtual void OnDisconnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge) {}
    }
}