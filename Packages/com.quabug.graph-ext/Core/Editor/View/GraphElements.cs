using System.Collections.Generic;
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
        public IEnumerable<TId> Ids { get; }
        public IEnumerable<TView> Views { get; }
    }

    public class GraphElements<TId, TView> : IReadOnlyGraphElements<TId, TView>
        where TId: struct
        where TView : GraphElement
    {
        private BiDictionary<TId, TView> Elements { get; } = new BiDictionary<TId, TView>();
        public TView this[in TId id] => Elements[id];
        public TId this[TView view] => Elements.GetKey(view);
        public bool Has(TId id) => Elements.ContainsKey(id);
        public bool Has(TView view) => Elements.ContainsValue(view);
        public void Add(TId id, TView view) => Elements.Add(id, view);
        public void Remove(TId id) => Elements.Remove(id);
        public void Remove(TView view) => Elements.RemoveValue(view);
        public IEnumerable<TId> Ids => Elements.Keys;
        public IEnumerable<TView> Views => Elements.Values;
    }
}