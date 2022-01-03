using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphExt
{
    [Serializable]
    public class FlatEdges
    {
        [SerializeField, HideInInspector] private List<SerializableEdge> _serializableEdges = new List<SerializableEdge>();
        private readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        public IReadOnlySet<EdgeId> GetEdges<TNode>(GraphRuntime<TNode> graph = null) where TNode : INode<GraphRuntime<TNode>>
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

        public void Connect<TNode>(in NodeId thisNodeId, in EdgeId edge, GraphRuntime<TNode> graph) where TNode : INode<GraphRuntime<TNode>>
        {
            if (!_edges.Contains(edge) && edge.Output.NodeId == thisNodeId)
            {
                _edges.Add(edge);
                var serializableEdge = edge.ToSerializable(graph);
                _serializableEdges.Add(serializableEdge);
            }
        }

        public void Disconnect(in NodeId thisNodeId, in EdgeId edge)
        {
            if (_edges.Contains(edge) && edge.Output.NodeId == thisNodeId)
            {
                _edges.Remove(edge);
                _serializableEdges.Remove(edge.ToSerializable());
            }
        }
    }
}