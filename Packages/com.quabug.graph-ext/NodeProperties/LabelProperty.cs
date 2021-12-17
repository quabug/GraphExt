using UnityEngine.UIElements;

namespace GraphExt
{
    public class LabelProperty : INodeProperty
    {
        public string Value;
        public LabelProperty(string value) => Value = value;

        public class Factory : NodePropertyViewFactory<LabelProperty>
        {
            protected override VisualElement Create(LabelProperty property, INodePropertyViewFactory _)
            {
                return new Label(property.Value);
            }
        }
    }
}