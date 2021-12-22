using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    internal static class VisualElementExtension
    {
        public static T FindAncestorElement<T>(this VisualElement self) where T : VisualElement
        {
            var parent = self?.parent;
            while (parent != null)
            {
                if (parent is T element) return element;
                parent = parent.parent;
            }
            return null;
        }
    }
}