using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Editor
{
    public class NodePositions<TNode> : IDictionary<NodeId, Vector2>, IReadOnlyDictionary<NodeId, Vector2>
    {
        [NotNull] private readonly IReadOnlyDictionary<NodeId, TNode> _nodeMap;
        [NotNull] private readonly Func<TNode, Vector2> _getPosition;
        [NotNull] private readonly Action<TNode, Vector2> _setPosition;

        public NodePositions(
            [NotNull] IReadOnlyDictionary<NodeId, TNode> nodeMap,
            [NotNull] Func<TNode, Vector2> getPosition,
            [NotNull] Action<TNode, Vector2> setPosition
        )
        {
            _nodeMap = nodeMap;
            _getPosition = getPosition;
            _setPosition = setPosition;
        }

        public Vector2 this[NodeId id]
        {
            get => _getPosition(_nodeMap[id]);
            set => _setPosition(_nodeMap[id], value);
        }

        private bool Has(in NodeId id)
        {
            return _nodeMap.ContainsKey(id);
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

        public void Clear()
        {
            // will never remove any position by this instance
        }

        public bool Contains(KeyValuePair<NodeId, Vector2> item)
        {
            return Has(item.Key);
        }

        public void CopyTo(KeyValuePair<NodeId, Vector2>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(KeyValuePair<NodeId, Vector2> item)
        {
            return Remove(item.Key);
        }

        public int Count => _nodeMap.Count;

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

        public ICollection<NodeId> Keys => _nodeMap.Keys.ToArray();
        public ICollection<Vector2> Values => _nodeMap.Values.Select(_getPosition).ToArray();
    }
}
