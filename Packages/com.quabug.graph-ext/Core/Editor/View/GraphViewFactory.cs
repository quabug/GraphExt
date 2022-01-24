using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public interface IGraphViewFactory
    {
        [NotNull] GraphView Create();
    }

    public class DefaultGraphViewFactory : IGraphViewFactory
    {
        [NotNull] private readonly IReadOnlyDictionary<Port, PortId> _ports;
        [NotNull] private readonly IsEdgeCompatibleFunc _isEdgeCompatible;

        public DefaultGraphViewFactory(
            [NotNull] IReadOnlyDictionary<Port, PortId> ports,
            [NotNull] IsEdgeCompatibleFunc isEdgeCompatible
        )
        {
            _ports = ports;
            _isEdgeCompatible= isEdgeCompatible;
        }

        public GraphView Create()
        {
            var graphView = new GraphView(GetCompatiblePorts);
            graphView.SetupGridBackground();
            graphView.SetupMiniMap();
            graphView.SetupDefaultManipulators();
            return graphView;
        }

        private IEnumerable<Port> GetCompatiblePorts(Port startPort)
        {
            foreach (var endPort in _ports.Keys)
            {
                if (startPort.orientation != endPort.orientation || startPort.direction == endPort.direction) continue;
                var startPortId = _ports[startPort];
                var endPortId = _ports[endPort];
                if (!_isEdgeCompatible(input: endPortId, output: startPortId)) continue;
                yield return endPort;
            }
        }
    }
}