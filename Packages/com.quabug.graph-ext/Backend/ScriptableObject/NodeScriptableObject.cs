using System;
using System.Collections.Generic;
using GraphExt.Editor;
using UnityEngine;

namespace GraphExt
{
    public class NodeScriptableObject<TNode> : ScriptableObject where TNode : INode<GraphRuntime<TNode>>
    {
        [SerializeReference, NodeProperty(CustomFactory = typeof(InnerNodeProperty.Factory))]
        public TNode Node;

        [SerializeField, HideInInspector] private List<SerializableEdge> _serializableEdges = new List<SerializableEdge>();
        private readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [SerializeField, NodeProperty(CustomFactory = typeof(NodeSerializedPositionProperty.Factory))]
        public Vector2 Position;

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

        public void OnConnected(GraphRuntime<TNode> graph, in EdgeId edge)
        {
            if (!_edges.Contains(edge))
            {
                _edges.Add(edge);
                var serializableEdge = edge.ToSerializable(graph);
                _serializableEdges.Add(serializableEdge);
            }
        }

        public void OnDisconnected(GraphRuntime<TNode> graph, in EdgeId edge)
        {
            if (_edges.Contains(edge))
            {
                _edges.Remove(edge);
                _serializableEdges.Remove(edge.ToSerializable());
            }
        }
    }
}