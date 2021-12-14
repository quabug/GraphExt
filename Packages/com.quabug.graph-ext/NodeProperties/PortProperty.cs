using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class PortProperty : INodeProperty
    {
        private readonly Type _type;
        private readonly Direction _direction;
        private readonly Port.Capacity _capacity;

        public PortProperty(Type type, Direction direction, Port.Capacity capacity)
        {
            _type = type;
            _direction = direction;
            _capacity = capacity;
        }

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory _)
            {
                return property is PortProperty port ? Port.Create<Edge>(Orientation.Horizontal, port._direction, port._capacity, port._type) : null;
            }
        }
    }
}
