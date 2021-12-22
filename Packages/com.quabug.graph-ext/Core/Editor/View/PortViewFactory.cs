using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public interface IPortViewFactory
    {
        Port CreatePort(in PortData data);
    }

    public class DefaultPortViewFactory : IPortViewFactory
    {
        public Port CreatePort(in PortData data)
        {
            var port = Port.Create<Edge>(data.Orientation.ToEditor(), data.Direction.ToEditor(), data.Capacity.ToEditor(), data.PortType);
            port.style.paddingLeft = 0;
            port.style.paddingRight = 0;
            return port;
        }
    }
}