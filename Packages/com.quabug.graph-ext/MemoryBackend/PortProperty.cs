using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Memory
{
    public class PortProperty : INodeProperty
    {
        private readonly Port _port;

        public PortProperty([NotNull] Port port)
        {
            _port = port;
        }

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory _)
            {
                return property is PortProperty port ? PortView.Create<Edge>(port._port) : null;
            }
        }
    }
}
