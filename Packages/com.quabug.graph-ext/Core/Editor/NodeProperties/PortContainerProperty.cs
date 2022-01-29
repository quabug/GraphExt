using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class PortContainerProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        public string PortName { get; }
        public PortContainerProperty(string portName) => PortName = portName;

        public class Factory : SingleNodePropertyViewFactory<PortContainerProperty>
        {
            protected override VisualElement CreateView(UnityEditor.Experimental.GraphView.Node node, PortContainerProperty property, Editor.INodePropertyViewFactory _)
            {
                return new PortContainer(property.PortName);
            }
        }
    }
}
