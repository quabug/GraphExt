using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Memory
{
    public class PortProperty : INodeProperty
    {
        private readonly IMemoryPort _port;
        private readonly Type _type;
        private readonly Direction _direction;
        private readonly UnityEditor.Experimental.GraphView.Port.Capacity _capacity;

        public PortProperty(IMemoryPort port, Type type, Direction direction, UnityEditor.Experimental.GraphView.Port.Capacity capacity)
        {
            _port = port;
            _type = type;
            _direction = direction;
            _capacity = capacity;
        }

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory _)
            {
                return property is PortProperty port ? CreatePort() : null;

                PortView CreatePort()
                {
                    var portModule = new Port(port._port, port._direction, port._capacity, port._type);
                    return PortView.Create<Edge>(portModule);
                }
            }
        }
    }
}
