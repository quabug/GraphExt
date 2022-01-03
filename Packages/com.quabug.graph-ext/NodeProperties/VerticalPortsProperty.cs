using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class VerticalPortsProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        public List<INodeProperty> Ports { get; } = new List<INodeProperty>();
        public string Name { get; set; }

        public class Factory : NodePropertyViewFactory<VerticalPortsProperty>
        {
            protected override VisualElement Create(Node node, VerticalPortsProperty property, INodePropertyViewFactory factory)
            {
                if (!property.Ports.Any()) return null;
                var container = new VisualElement
                {
                    style =
                    {
                        flexGrow = new StyleFloat(1),
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                        justifyContent = new StyleEnum<Justify>(Justify.SpaceAround)
                    }
                };
                if (!string.IsNullOrEmpty(property.Name)) container.name = property.Name;
                container.AddToClassList("vertical-ports");
                foreach (var port in property.Ports.Select(port => factory.Create(node, port, factory))) container.Add(port);
                return container;
            }
        }
    }
}