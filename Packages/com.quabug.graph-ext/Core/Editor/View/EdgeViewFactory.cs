using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public interface IEdgeViewFactory
    {
        Edge CreateEdge(Port port1, Port port2);
        void AfterCreated(Edge edge);
    }

    public class DefaultEdgeViewFactory : IEdgeViewFactory
    {
        public Edge CreateEdge(Port port1, Port port2)
        {
            var edge = port1.ConnectTo(port2);
            AfterCreated(edge);
            return edge;
        }

        public void AfterCreated(Edge edge)
        {
            edge.showInMiniMap = true;
        }
    }
}