using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    /// <summary>
    /// A minimal `GraphView` implementation
    /// </summary>
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        [NotNull] private readonly IsEdgeCompatibleFunc _isEdgeCompatible;
        [NotNull] private readonly IReadOnlyDictionary<Port, PortId> _ports;

        public GraphView(
            [NotNull] IsEdgeCompatibleFunc isEdgeCompatible,
            [NotNull] IReadOnlyDictionary<Port, PortId> ports
        )
        {
            _isEdgeCompatible = isEdgeCompatible;
            _ports = ports;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(endPort =>
            {
                if (startPort.orientation != endPort.orientation || startPort.direction == endPort.direction) return;
                var startPortId = _ports[startPort];
                var endPortId = _ports[endPort];
                if (!_isEdgeCompatible(input: endPortId, output: startPortId)) return;
                compatiblePorts.Add(endPort);
            });
            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.StopPropagation();
        }
    }

    public static class GraphViewExtension
    {
        public static void SetupGridBackground(this UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            graphView.Insert(0, new GridBackground { name = "grid" });
        }

        public static void SetupDefaultManipulators(this UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            graphView.AddManipulator(new ContentZoomer());
            graphView.AddManipulator(new ContentDragger());
            graphView.AddManipulator(new SelectionDragger());
            graphView.AddManipulator(new RectangleSelector());
        }

        public static void SetupMiniMap(this UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            var miniMap = new MiniMap();
            graphView.Add(miniMap);
            // NOTE: not working... have to set `graphView` on `CreateGUI` of `Window`
            miniMap.graphView = graphView;
            miniMap.windowed = true;
            miniMap.name = "minimap";
        }
    }
}