using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    [CanBeNull] internal delegate TView CreateView<in TData, out TView>(TData data);
    internal delegate void RemoveView<in TId>(TId data);

    internal class GraphElements<TId, TData, TView>
        where TId : struct
        where TView : VisualElement
    {
        [NotNull] private readonly CreateView<TData, TView> _createView;
        [NotNull] private readonly RemoveView<TId> _removeView;

        public BiDictionary<TId, TView> Elements { get; } = new BiDictionary<TId, TView>();
        private readonly HashSet<TId> _cache = new HashSet<TId>();

        public GraphElements([NotNull] CreateView<TData, TView> createView, [NotNull] RemoveView<TId> removeView)
        {
            _createView = createView;
            _removeView = removeView;
        }

        public void UpdateElements(IEnumerable<(TId id, TData data)> newElements)
        {
            _cache.Clear();
            foreach (var old in Elements.Keys) _cache.Add(old);

            foreach (var (newId, newData) in newElements)
            {
                if (Elements.ContainsKey(newId))
                {
                    _cache.Remove(newId);
                }
                else
                {
                    var element = _createView(newData);
                    if (element != null) Elements.Add(newId, element);
                }
            }

            foreach (var removed in _cache)
            {
                _removeView(removed);
                Elements.Remove(removed);
            }
        }
    }
}