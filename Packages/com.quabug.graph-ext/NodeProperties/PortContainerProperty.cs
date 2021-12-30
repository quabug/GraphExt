using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class PortContainerProperty : INodeProperty
    {
        public int Order => 0;
        public PortId PortId { get; }
        public PortContainerProperty(PortId portId) => PortId = portId;

        public class Factory : NodePropertyViewFactory<PortContainerProperty>
        {
            protected override VisualElement Create(UnityEditor.Experimental.GraphView.Node node, PortContainerProperty property, Editor.INodePropertyViewFactory _)
            {
                return new PortContainer(property.PortId);
            }
        }
    }
}
