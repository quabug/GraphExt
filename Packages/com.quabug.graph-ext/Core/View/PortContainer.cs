using UnityEngine.UIElements;

namespace GraphExt
{
    public class PortContainer : VisualElement
    {
        public PortId PortId { get; }
        public PortContainer(PortId portId) => PortId = portId;
    }
}