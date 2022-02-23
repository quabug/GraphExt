using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class ToggleDisplayProperty : INodeProperty
    {
        public INodeProperty InnerProperty { get; set; }
        public int Order { get; set; } = 0;

        public bool IsHidden
        {
            get => _hidden;
            set
            {
                _hidden = value;
                _valueChange?.Invoke(value);
            }
        }

        private bool _hidden = true;
        private event Action<bool> _valueChange;

        private class View : VisualElement
        {
            public View(ToggleDisplayProperty property)
            {
                SetDisplay(property._hidden);
                property._valueChange += SetDisplay;
                void SetDisplay(bool isHidden) => style.display = isHidden ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        private class ViewFactory : SingleNodePropertyViewFactory<ToggleDisplayProperty>
        {
            protected override VisualElement CreateView(Node node, ToggleDisplayProperty property, INodePropertyViewFactory factory)
            {
                var view = new View(property);
                foreach(var innerView in factory.Create(node, property.InnerProperty, factory)) view.Add(innerView);
                return view;
            }
        }
    }
}