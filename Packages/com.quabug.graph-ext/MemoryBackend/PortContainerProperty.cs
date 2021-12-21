using UnityEngine.UIElements;

namespace GraphExt.Memory
{
    public class PortContainerProperty : INodeProperty
    {
        public PortId PortId { get; }
        public PortContainerProperty(PortId portId) => PortId = portId;

#if UNITY_EDITOR
        public class Factory : NodePropertyViewFactory<PortContainerProperty>
        {
            protected override VisualElement Create(PortContainerProperty property, INodePropertyViewFactory _)
            {
                return new Editor.PortContainer(property.PortId);
            }
        }
#endif
    }
}
