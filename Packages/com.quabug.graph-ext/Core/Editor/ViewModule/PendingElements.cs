using System.Collections.Generic;

namespace GraphExt.Editor
{
    /// <summary>
    /// Elements waiting for update.
    /// </summary>
    /// <typeparam name="TData">Data of element. <see cref="INode{TGraph}"/>, <see cref="EdgeId"/> etc.</typeparam>
    public class PendingElements<TData>
    {
        public readonly HashSet<TData> Value = new HashSet<TData>();
    }

    /// <summary>
    /// Elements waiting for update.
    /// </summary>
    /// <typeparam name="TId">Data Id of element. <see cref="NodeId"/>, <see cref="PortId"/>, <see cref="EdgeId"/> etc.</typeparam>
    /// <typeparam name="TData">Data of element. <see cref="INode{TGraph}"/>, <see cref="EdgeId"/> etc.</typeparam>
    public class PendingElements<TId, TData>
    {
        public readonly Dictionary<TId, TData> Value = new Dictionary<TId, TData>();
        public TData this[in TId id] => Value[id];
    }
}