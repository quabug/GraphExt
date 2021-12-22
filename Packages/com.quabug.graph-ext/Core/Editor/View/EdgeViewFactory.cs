using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public interface IEdgeViewFactory
    {
        Edge CreateEdge(Port port1, Port port2);
    }

    public class DefaultEdgeViewFactory : IEdgeViewFactory
    {
        public Edge CreateEdge(Port port1, Port port2)
        {
            return port1.ConnectTo(port2);
        }
    }
}