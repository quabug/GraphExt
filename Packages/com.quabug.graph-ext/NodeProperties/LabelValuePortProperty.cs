using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class LabelValuePortProperty : INodeProperty
    {
        public INodeProperty LabelProperty { get; }
        public INodeProperty ValueProperty { get; }
        public INodeProperty LeftPort { get; }
        public INodeProperty RightPort { get; }

        public LabelValuePortProperty(INodeProperty labelProperty, INodeProperty valueProperty, INodeProperty leftPort, INodeProperty rightPort)
        {
            LabelProperty = labelProperty;
            ValueProperty = valueProperty;
            LeftPort = leftPort;
            RightPort = rightPort;
        }

        public class Factory : NodePropertyViewFactory<LabelValuePortProperty>
        {
            protected override VisualElement Create(LabelValuePortProperty property, INodePropertyViewFactory factory)
            {
                var container = new VisualElement();

                var label = factory.Create(property.LabelProperty, factory) ?? new VisualElement();
                var value = factory.Create(property.ValueProperty, factory) ?? new VisualElement();
                var leftPort = factory.Create(property.LeftPort, factory) ?? new VisualElement();
                var rightPort = factory.Create(property.RightPort, factory) ?? new VisualElement();

                container.name = "label-value-property";
                container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                container.style.height = 20;

                label.name = "label-property";
                label.style.width = 60;
                label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft);

                value.name = "value-property";
                value.style.flexGrow = 1;

                leftPort.name = "left-port";
                leftPort.style.width = 20;

                rightPort.name = "right-port";
                rightPort.style.width = 20;

                container.Add(leftPort);
                container.Add(label);
                container.Add(value);
                container.Add(rightPort);

                return container;
            }
        }
    }
}