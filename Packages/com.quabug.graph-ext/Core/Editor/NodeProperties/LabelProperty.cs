using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class LabelProperty : INodeProperty
    {
        public int Order { get; set; } = 0;

        private readonly string _text;
        private readonly string _name;
        private string[] _classes;

        public LabelProperty(string text, string name = null, params string[] classes)
        {
            _text = text;
            _name = name;
            _classes = classes ?? Array.Empty<string>();
        }

        public class Factory : SingleNodePropertyViewFactory<LabelProperty>
        {
            protected override VisualElement CreateView(Node node, LabelProperty property, INodePropertyViewFactory _)
            {
                var label = new Label(property._text);
                label.name = property._name;
                foreach (var @class in property._classes) label.AddToClassList(@class);
                return label;
            }
        }
    }
}