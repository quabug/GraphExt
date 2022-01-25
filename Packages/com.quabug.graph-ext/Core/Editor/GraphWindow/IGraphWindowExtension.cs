using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public interface IGraphWindowExtension
    {
        void RecreateGraphView([NotNull] VisualElement root);
        void Tick();
        void Clear();
    }
}