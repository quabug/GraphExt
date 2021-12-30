using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class VerticalPortsProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        public List<INodeProperty> Ports = new List<INodeProperty>();

        public class Factory : NodePropertyViewFactory<VerticalPortsProperty>
        {
            protected override VisualElement Create(Node node, VerticalPortsProperty property, INodePropertyViewFactory factory)
            {
                if (!property.Ports.Any()) return null;
                var container = new VisualElement
                {
                    style =
                    {
                        alignSelf = new StyleEnum<Align>(Align.Center),
                        alignItems = new StyleEnum<Align>(Align.Center),
                        alignContent = new StyleEnum<Align>(Align.Center),
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                    }
                };
                foreach (var port in property.Ports.Select(port => factory.Create(node, port, factory))) container.Add(port);
                return container;
            }
        }
    }
}