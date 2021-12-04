using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace GraphExt
{
    public interface INodePropertyViewFactory
    {
        [CanBeNull] VisualElement Create([NotNull] INodeProperty property);
    }
}