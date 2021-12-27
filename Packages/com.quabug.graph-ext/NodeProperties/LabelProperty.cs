using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class LabelProperty : INodeProperty
    {
        public string Value;
        public LabelProperty(string value) => Value = value;

        public class Factory : NodePropertyViewFactory<LabelProperty>
        {
            protected override VisualElement Create(Node node, LabelProperty property, INodePropertyViewFactory _)
            {
                return new Label(property.Value);
            }
        }
    }
}