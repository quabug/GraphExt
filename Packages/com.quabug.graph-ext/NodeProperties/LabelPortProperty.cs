using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class LabelPortProperty : INodeProperty
    {
        public INodeProperty LabelProperty { get; }
        public INodeProperty InputPortProperty { get; }
        public INodeProperty OutputPortProperty { get; }

        public IEnumerable<IPortModule> Ports
        {
            get
            {
                var ports = Enumerable.Empty<IPortModule>();
                if (InputPortProperty != null) ports = ports.Concat(InputPortProperty.Ports);
                if (OutputPortProperty != null) ports = ports.Concat(OutputPortProperty.Ports);
                return ports;
            }
        }

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

                    var input = CreatePort(labelPort.InputPortProperty);
                    input.name = "input-port";
                    container.Add(input);

                    var label = factory.Create(labelPort.LabelProperty, factory);
                    label.name = "label-property";
                    label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
                    container.Add(label);

                    var output = CreatePort(labelPort.OutputPortProperty);
                    output.name = "output-port";
                    container.Add(output);

                    return container;
                }
                return null;

                VisualElement CreatePort(INodeProperty portProperty)
                {
                    VisualElement port;
                    if (portProperty == null) port = new VisualElement();
                    else port = factory.Create(portProperty, factory) ?? new VisualElement();
                    port.style.width = 40;
                    return port;
                }
            }
        }
    }
}
