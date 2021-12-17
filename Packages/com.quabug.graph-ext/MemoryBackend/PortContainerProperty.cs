using System;
using UnityEngine.UIElements;

namespace GraphExt.Memory
{
    public class PortContainerProperty : INodeProperty
    {
        public PortId PortId { get; }
        public PortContainerProperty(PortId portId) => PortId = portId;

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory _)
            {
                return property is PortContainerProperty port ? new PortContainer(port.PortId) : null;
            }
        }
    }
}
