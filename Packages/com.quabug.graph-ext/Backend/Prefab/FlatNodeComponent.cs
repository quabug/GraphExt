using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphExt
{
    [DisallowMultipleComponent, AddComponentMenu("")]
    public class FlatNodeComponent<TNode, TComponent> : MonoBehaviour, INodeComponent<TNode, TComponent>
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : FlatNodeComponent<TNode, TComponent>
    {
        [SerializeReference] private TNode _node;
        public TNode Node { get => _node; set => _node = value; }
        public string NodeSerializedPropertyName => nameof(_node);

        [SerializeField, HideInInspector] private List<SerializableEdge> _serializableEdges = new List<SerializableEdge>();
        private readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [field: SerializeField, HideInInspector] public Vector2 Position { get; set; }

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<TNode> graph)
        {
            _edges.Clear();
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
            return _edges;
        }

        public bool IsPortCompatible(GameObjectNodes<TNode, TComponent> data, in PortId input, in PortId output)
        {
            return true;
        }

        public void OnConnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            if (!_edges.Contains(edge))
            {
                _edges.Add(edge);
                var serializableEdge = edge.ToSerializable(graph.Graph);
                _serializableEdges.Add(serializableEdge);
            }
        }

        public void OnDisconnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            if (_edges.Contains(edge))
            {
                _edges.Remove(edge);
                _serializableEdges.Remove(edge.ToSerializable());
            }
        }
    }
}