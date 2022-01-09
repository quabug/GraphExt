using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        [NotNull] private readonly IEdgeConnectionViewModule _edgeConnectionViewModule;
        [NotNull] private readonly IReadOnlyGraphElements<PortId, Port> _ports;

        public GraphView(
            [NotNull] IEdgeConnectionViewModule edgeConnectionViewModule,
            [NotNull] IReadOnlyGraphElements<PortId, Port> ports
        )
        {
            _edgeConnectionViewModule = edgeConnectionViewModule;
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
                if (!_edgeConnectionViewModule.IsCompatible(input: endPortId, output: startPortId)) return;
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