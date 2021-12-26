using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class DynamicTitleProperty : INodeProperty
    {
        public Func<string> GetName;
        public DynamicTitleProperty(Func<string> getName) => GetName = getName;

#if UNITY_EDITOR
        private class View : Label, Editor.ITickableElement
        {
            private readonly Func<string> _getName;

            public View(Func<string> getName)
            {
                _getName = getName;
                style.height = 40;
                style.flexGrow = 1;
                style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
                style.fontSize = new StyleLength(14);
                style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
                UpdateName();
            }

            public void Tick()
            {
                UpdateName();
            }

            void UpdateName()
            {
                text = _getName();
                style.display = new StyleEnum<DisplayStyle>(string.IsNullOrEmpty(text) ? DisplayStyle.None : DisplayStyle.Flex);
            }
        }

        private class Factory : Editor.NodePropertyViewFactory<DynamicTitleProperty>
        {
            protected override VisualElement Create(UnityEditor.Experimental.GraphView.Node node, DynamicTitleProperty property, Editor.INodePropertyViewFactory _)
            {
                return new View(property.GetName);
            }
        }
#endif
    }
}
