using System;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class DynamicTitleProperty : INodeProperty
    {
        public int Order { get; set; } = -1;

        public Func<string> GetName;
        public DynamicTitleProperty(Func<string> getName) => GetName = getName;

        private class View : TitleLabel, ITickableElement
        {
            private readonly Func<string> _getName;

            public View(Func<string> getName)
            {
                _getName = getName;
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

        private class ViewFactory : SingleNodePropertyViewFactory<DynamicTitleProperty>
        {
            protected override VisualElement CreateView(UnityEditor.Experimental.GraphView.Node node, DynamicTitleProperty property, Editor.INodePropertyViewFactory _)
            {
                return new View(property.GetName);
            }
        }
    }
}
