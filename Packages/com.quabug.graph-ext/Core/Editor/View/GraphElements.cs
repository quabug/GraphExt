using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class GraphElements<TId, TView>
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
        public ICollection<TId> Ids => Elements.Keys;
        public ICollection<TView> Views => Elements.Values;
    }
}