using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public interface IPortViewFactory : IGraphElementViewFactory
    {
        Port CreatePort(in PortData data);
    }

    public class DefaultPortViewFactory : IPortViewFactory
    {
        public Port CreatePort(in PortData data)
        {
            var port = Port.Create<Edge>(data.Orientation.ToEditor(), data.Direction.ToEditor(), data.Capacity > 1 ? Port.Capacity.Multi : Port.Capacity.Single, data.PortType);
            port.showInMiniMap = true;
            port.style.paddingLeft = 0;
            port.style.paddingRight = 0;
            foreach (var @class in data.AdditionalClasses) port.AddToClassList(@class);
            return port;
        }
    }
}