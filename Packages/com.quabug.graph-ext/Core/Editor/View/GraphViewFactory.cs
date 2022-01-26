using JetBrains.Annotations;

namespace GraphExt.Editor
{
    public interface IGraphViewFactory
    {
        [NotNull] GraphView Create(GraphView.FindCompatiblePorts findCompatiblePorts);
    }

    public class DefaultGraphViewFactory : IGraphViewFactory
    {
        public GraphView Create(GraphView.FindCompatiblePorts findCompatiblePorts)
        {
            var graphView = new GraphView(findCompatiblePorts);
            graphView.SetupGridBackground();
            graphView.SetupDefaultManipulators();
            graphView.SetupMiniMap();
            return graphView;
        }
    }
}