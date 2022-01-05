using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class PortContainer : VisualElement
    {
        public string PortName { get; }
        private Port _port = null;

        public PortContainer(string portName)
        {
            name = portName;
            PortName = portName;
        }

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