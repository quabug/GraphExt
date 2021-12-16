using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class LabelValueProperty : INodeProperty
    {
        public INodeProperty LabelProperty { get; }
        public INodeProperty ValueProperty { get; }

        public LabelValueProperty(INodeProperty labelProperty, INodeProperty valueProperty)
        {
            LabelProperty = labelProperty;
            ValueProperty = valueProperty;
        }

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory factory)
            {
                if (property is LabelValueProperty labelValue)
                {
                    var container = new VisualElement();
                    container.name = "label-value-property";
                    container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    container.style.marginLeft = 10;
                    container.style.marginRight = 10;

                    var label = factory.Create(labelValue.LabelProperty, factory);
                    label.name = "label-property";
                    label.style.width = 60;
                    label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft);
                    container.Add(label);

                    var value = factory.Create(labelValue.ValueProperty, factory);
                    value.name = "value-property";
                    value.style.flexGrow = 1;
                    container.Add(value);

                    return container;
                }
                return null;
            }
        }
    }
}