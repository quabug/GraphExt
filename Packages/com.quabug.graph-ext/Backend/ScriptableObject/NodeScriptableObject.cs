using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphExt
{
    public class NodeScriptableObject : ScriptableObject
    {
        [SerializeField, HideInInspector] protected List<SerializableEdge> _serializableEdges = new List<SerializableEdge>();
        protected readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

#if UNITY_EDITOR
        [SerializeField, NodeProperty(CustomFactory = typeof(Editor.NodeSerializedPositionProperty.Factory))]
#endif
        public Vector2 Position;
    }

    public class NodeScriptableObject<TNode> : NodeScriptableObject where TNode : INode<GraphRuntime<TNode>>
    {
#if UNITY_EDITOR
        [SerializeReference, NodeProperty(CustomFactory = typeof(Editor.InnerNodeProperty.Factory))]
#endif
        public TNode Node;

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