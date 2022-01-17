using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class DynamicTitleProperty : INodeProperty
    {
        public int Order => 0;

        public Func<string> GetName;
        public DynamicTitleProperty(Func<string> getName) => GetName = getName;

        private class View : Label, ITickableElement
        {
            private readonly Func<string> _getName;

            public View(Func<string> getName)
            {
                _getName = getName;
                name = "title-property";
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

        private class Factory : SingleNodePropertyViewFactory<DynamicTitleProperty>
        {
            protected override VisualElement CreateView(UnityEditor.Experimental.GraphView.Node node, DynamicTitleProperty property, Editor.INodePropertyViewFactory _)
            {
                return new View(property.GetName);
            }
        }
    }
}
