using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public interface IReadOnlyGraphElements<TId, TView>
        where TId: struct
        where TView : GraphElement
    {
        public TView this[in TId id] { get; }
        public TId this[TView view] { get; }
        public bool Has(TId id);
        public bool Has(TView view);
        public IEnumerable<(TId id, TView view)> Elements { get; }
    }

    public interface IGraphElements<TId, TView> : IReadOnlyGraphElements<TId, TView>
        where TId: struct
        where TView : GraphElement
    {
        public void Add(TId id, TView view);
        public void Remove(TId id);
        public void Remove(TView view);
    }

    public class GraphElements<TId, TView> : IGraphElements<TId, TView>
        where TId: struct
        where TView : GraphElement
    {
        private readonly BiDictionary<TId, TView> _elements = new BiDictionary<TId, TView>();
        public TView this[in TId id] => _elements[id];
        public TId this[TView view] => _elements.GetKey(view);
        public bool Has(TId id) => _elements.ContainsKey(id);
        public bool Has(TView view) => _elements.ContainsValue(view);
        public void Add(TId id, TView view) => _elements.Add(id, view);
        public void Remove(TId id) => _elements.Remove(id);
        public void Remove(TView view) => _elements.RemoveValue(view);
        public IEnumerable<(TId id, TView view)> Elements => _elements.Select(pair => (pair.Key, pair.Value));
    }
}