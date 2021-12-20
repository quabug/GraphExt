using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class PortContainer : VisualElement
    {
        public PortId PortId { get; }
        private Port _port = null;

        public PortContainer(PortId portId) => PortId = portId;

        public void AddPort([NotNull] Port port)
        {
            Assert.IsNull(_port);
            _port = port;
            Add(port);
        }

        [NotNull] public Port RemovePort()
        {
            Assert.IsNotNull(_port);
            Remove(_port);
            var port = _port;
            _port = null;
            return port;
        }
    }
}