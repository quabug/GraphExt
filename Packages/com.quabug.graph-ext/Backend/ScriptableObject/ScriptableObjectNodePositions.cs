using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt
{
    public class ScriptableObjectNodePositions<TNode, TNodeScriptableObject> : IDictionary<NodeId, Vector2>, IReadOnlyDictionary<NodeId, Vector2>
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        private readonly GraphScriptableObject<TNode, TNodeScriptableObject> _graph;
        public ScriptableObjectNodePositions(GraphScriptableObject<TNode, TNodeScriptableObject> graph)
        {
            _graph = graph;
        }

        public Vector2 this[NodeId id]
        {
            get => _graph[id].Position;
            set => _graph[id].Position = value;
        }

        private bool Has(in NodeId id)
        {
            return _graph.NodeObjectMap.ContainsKey(id);
        }

        public IEnumerator<KeyValuePair<NodeId, Vector2>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<NodeId, Vector2> item)
        {
            if (Has(item.Key)) this[item.Key] = item.Value;
        }

        public void Clear() {}

        public bool Contains(KeyValuePair<NodeId, Vector2> item)
        {
            return Has(item.Key);
        }

        public void CopyTo(KeyValuePair<NodeId, Vector2>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<NodeId, Vector2> item)
        {
            return Remove(item.Key);
        }

        public int Count => _graph.Nodes.Count;

        public bool IsReadOnly => true;

        public void Add(NodeId key, Vector2 value)
        {
            if (Has(key)) this[key] = value;
        }

        public bool ContainsKey(NodeId key)
        {
            return Has(key);
        }

        public bool Remove(NodeId key)
        {
            if (Has(key)) this[key] = Vector2.zero;
            return true;
        }

        public bool TryGetValue(NodeId key, out Vector2 value)
        {
            var hasNode = Has(key);
            value = hasNode ? this[key] : Vector2.zero;
            return hasNode;
        }

        IEnumerable<NodeId> IReadOnlyDictionary<NodeId, Vector2>.Keys => Keys;
        IEnumerable<Vector2> IReadOnlyDictionary<NodeId, Vector2>.Values => Values;

        public ICollection<NodeId> Keys => _graph.Nodes.Select(node => node.Id).ToArray();
        public ICollection<Vector2> Values => _graph.Nodes.Select(node => node.Position).ToArray();
    }
}