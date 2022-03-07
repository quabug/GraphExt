using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class EventTitleProperty : INodeProperty
    {
        public int Order { get; set; } = -1;
        private string _title;
        public string Title
        {
            private get => _title;
            set
            {
                _title = value;
                TitleChanged?.Invoke(_title);
            }
        }

        private event Action<string> TitleChanged;

        public class View : TitleLabel
        {
            public View(EventTitleProperty property) : base(property.Title)
            {
                property.TitleChanged += title =>
                {
                    var displayStyle = title == null ? DisplayStyle.None : DisplayStyle.Flex;
                    style.display = new StyleEnum<DisplayStyle>(displayStyle);
                    text = title;
                };
            }
        }

        [UsedImplicitly]
        private class ViewFactory : SingleNodePropertyViewFactory<EventTitleProperty>
        {
            protected override VisualElement CreateView(Node node, EventTitleProperty property, INodePropertyViewFactory factory)
            {
                return new View(property);
            }
        }
    }
}