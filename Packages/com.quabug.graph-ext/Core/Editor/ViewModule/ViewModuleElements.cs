using System.Collections.Generic;
using System.Linq;

namespace GraphExt.Editor
{
    public interface IReadOnlyViewModuleElements<TData>
    {
        IEnumerable<TData> Elements { get; }
        bool Has(TData data);
    }

    public interface IViewModuleElements<TData> : IReadOnlyViewModuleElements<TData>
    {
        void Add(TData data);
        void Remove(TData data);
    }

    /// <summary>
    /// View module of elements.
    /// </summary>
    /// <typeparam name="TData">Data of element. <see cref="INode{TGraph}"/>, <see cref="EdgeId"/> etc.</typeparam>
    public class ViewModuleElements<TData> : IViewModuleElements<TData>
    {
        public readonly HashSet<TData> Value;
        public ViewModuleElements() => Value = new HashSet<TData>();
        public ViewModuleElements(IEnumerable<TData> elements) => Value = elements.ToHashSet();
        public IEnumerable<TData> Elements => Value;
        public bool Has(TData data) => Value.Contains(data);
        public void Add(TData data) => Value.Add(data);
        public void Remove(TData data) => Value.Remove(data);
    }

    public interface IReadOnlyViewModuleElements<TId, TData>
    {
        TData this[in TId id] { get; }
        bool Has(TId id);
        bool Has(TData data);
        IEnumerable<TId> Ids { get; }
        IEnumerable<TData> Datas { get; }
        IEnumerable<(TId id, TData data)> Elements { get; }

    }

    public interface IViewModuleElements<TId, TData> : IReadOnlyViewModuleElements<TId, TData>
    {
        new TData this[in TId id] { get; set; }
        void Add(in TId id, TData data);
        void Remove(in TId id);
    }

    /// <summary>
    /// View module of elements.
    /// </summary>
    /// <typeparam name="TId">Data Id of element. <see cref="NodeId"/>, <see cref="PortId"/>, <see cref="EdgeId"/> etc.</typeparam>
    /// <typeparam name="TData">Data of element. <see cref="INode{TGraph}"/>, <see cref="EdgeId"/> etc.</typeparam>
    public class ViewModuleElements<TId, TData> : IViewModuleElements<TId, TData>
    {
        public ViewModuleElements() => Value = new Dictionary<TId, TData>();
        public ViewModuleElements(IReadOnlyDictionary<TId, TData> elements) => Value = elements.ToDictionary(e => e.Key, e => e.Value);

        public readonly Dictionary<TId, TData> Value;

        public TData this[in TId id]
        {
            get => Value[id];
            set => Value[id] = value;
        }

        public void Add(in TId id, TData data) => Value.Add(id, data);
        public void Remove(in TId id) => Value.Remove(id);

        public bool Has(TId id) => Value.ContainsKey(id);
        public bool Has(TData data) => Value.ContainsValue(data);
        public IEnumerable<TId> Ids => Value.Keys;
        public IEnumerable<TData> Datas => Value.Values;
        public IEnumerable<(TId id, TData data)> Elements => Value.Select(pair => (pair.Key, pair.Value));
    }
}
