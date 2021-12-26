using UnityEngine.UIElements;

namespace GraphExt
{
    public class PortContainerProperty : INodeProperty
    {
        public PortId PortId { get; }
        public PortContainerProperty(PortId portId) => PortId = portId;

#if UNITY_EDITOR
        public class Factory : Editor.NodePropertyViewFactory<PortContainerProperty>
        {
            protected override VisualElement Create(UnityEditor.Experimental.GraphView.Node node, PortContainerProperty property, Editor.INodePropertyViewFactory _)
            {
                return new Editor.PortContainer(property.PortId);
            }
        }
#endif
    }
}
