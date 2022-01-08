using System.Collections.Generic;

namespace GraphExt.Editor
{
    /// <summary>
    /// View module of elements.
    /// </summary>
    /// <typeparam name="TData">Data of element. <see cref="INode{TGraph}"/>, <see cref="EdgeId"/> etc.</typeparam>
    public class ViewModuleElements<TData>
    {
        public readonly HashSet<TData> Value = new HashSet<TData>();
    }

    /// <summary>
    /// View module of elements.
    /// </summary>
    /// <typeparam name="TId">Data Id of element. <see cref="NodeId"/>, <see cref="PortId"/>, <see cref="EdgeId"/> etc.</typeparam>
    /// <typeparam name="TData">Data of element. <see cref="INode{TGraph}"/>, <see cref="EdgeId"/> etc.</typeparam>
    public class ViewModuleElements<TId, TData>
    {
        public readonly Dictionary<TId, TData> Value = new Dictionary<TId, TData>();
        public TData this[in TId id] => Value[id];
    }
}
