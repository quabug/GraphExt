using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class LabelPortProperty : INodeProperty
    {
        public INodeProperty LabelProperty { get; }
        public INodeProperty InputPortProperty { get; }
        public INodeProperty OutputPortProperty { get; }

        public LabelPortProperty(INodeProperty labelProperty, INodeProperty inputPortProperty, INodeProperty outputPortProperty)
        {
            LabelProperty = labelProperty;
            InputPortProperty = inputPortProperty;
            OutputPortProperty = outputPortProperty;
        }

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory factory)
            {
                if (property is LabelPortProperty labelPort)
                {
                    var container = new VisualElement();
                    container.name = "label-port-property";
                    container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    container.style.justifyContent = new StyleEnum<Justify>(Justify.SpaceBetween);

                    var input = CreatePortContainer(labelPort.InputPortProperty);
                    input.name = "input-port";
                    container.Add(input);

                    var label = factory.Create(labelPort.LabelProperty, factory);
                    label.name = "label-property";
                    label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
                    container.Add(label);

                    var output = CreatePortContainer(labelPort.OutputPortProperty);
                    output.name = "output-port";
                    container.Add(output);

                    return container;
                }
                return null;

                VisualElement CreatePortContainer(INodeProperty portProperty)
                {
                    var port = factory.Create(portProperty, factory) ?? new VisualElement();
                    port.style.width = 40;
                    return port;
                }
            }
        }
    }
}
