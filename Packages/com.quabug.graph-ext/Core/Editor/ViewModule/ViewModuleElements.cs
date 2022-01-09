using System.Collections.Generic;
using System.Linq;

namespace GraphExt.Editor
{
    public interface IReadOnlyViewModuleElements<TData>
    {
        public IReadOnlySet<TData> Value { get; }
    }

    /// <summary>
    /// View module of elements.
    /// </summary>
    /// <typeparam name="TData">Data of element. <see cref="INode{TGraph}"/>, <see cref="EdgeId"/> etc.</typeparam>
    public class ViewModuleElements<TData> : IReadOnlyViewModuleElements<TData>
    {
        public readonly HashSet<TData> Value;
        IReadOnlySet<TData> IReadOnlyViewModuleElements<TData>.Value => Value;

        public ViewModuleElements() => Value = new HashSet<TData>();
        public ViewModuleElements(IEnumerable<TData> elements) => Value = elements.ToHashSet();
    }

    public interface IReadOnlyViewModuleElements<TId, TData>
    {
        public IReadOnlyDictionary<TId, TData> Value { get; }
        public TData this[in TId id] { get; }
    }

    /// <summary>
    /// View module of elements.
    /// </summary>
    /// <typeparam name="TId">Data Id of element. <see cref="NodeId"/>, <see cref="PortId"/>, <see cref="EdgeId"/> etc.</typeparam>
    /// <typeparam name="TData">Data of element. <see cref="INode{TGraph}"/>, <see cref="EdgeId"/> etc.</typeparam>
    public class ViewModuleElements<TId, TData> : IReadOnlyViewModuleElements<TId, TData>
    {
        public readonly Dictionary<TId, TData> Value;
        public TData this[in TId id]
        {
            get => Value[id];
            set => Value[id] = value;
        }
        IReadOnlyDictionary<TId, TData> IReadOnlyViewModuleElements<TId, TData>.Value => Value;

        public ViewModuleElements() => Value = new Dictionary<TId, TData>();
        public ViewModuleElements(IReadOnlyDictionary<TId, TData> elements) => Value = elements.ToDictionary(e => e.Key, e => e.Value);
    }
}
