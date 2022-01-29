using System;
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

        class View : TitleLabel
        {
            public View(EventTitleProperty property) : base(property.Title)
            {
                property.TitleChanged += title => text = title;
            }
        }

        public class ViewFactory : SingleNodePropertyViewFactory<EventTitleProperty>
        {
            protected override VisualElement CreateView(Node node, EventTitleProperty property, INodePropertyViewFactory factory)
            {
                return new View(property);
            }
        }
    }
}