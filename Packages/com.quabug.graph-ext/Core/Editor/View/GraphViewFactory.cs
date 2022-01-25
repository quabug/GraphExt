using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

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

        public static GraphView.FindCompatiblePorts GetFindCompatiblePortsFunc(
            [NotNull] IReadOnlyDictionary<Port, PortId> ports,
            [NotNull] IsEdgeCompatibleFunc isEdgeCompatible
        )
        {
            return GetCompatiblePorts;

            IEnumerable<Port> GetCompatiblePorts(Port startPort)
            {
                foreach (var endPort in ports.Keys)
                {
                    if (startPort.orientation != endPort.orientation || startPort.direction == endPort.direction) continue;
                    var startPortId = ports[startPort];
                    var endPortId = ports[endPort];
                    if (!isEdgeCompatible(input: endPortId, output: startPortId)) continue;
                    yield return endPort;
                }
            }
        }
    }
}