using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
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

            Insert(0, new GridBackground { name = "grid" });

            var miniMap = new MiniMap();
            Add(miniMap);
            // NOTE: not working... have to set `graphView` on `CreateGUI` of `BehaviorTreeEditor`
            miniMap.graphView = this;
            miniMap.windowed = true;
            miniMap.name = "minimap";

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
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
}