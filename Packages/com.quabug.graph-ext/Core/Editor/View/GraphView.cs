using System.Collections.Generic;
using System.Linq;
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
        public delegate IEnumerable<Port> FindCompatiblePorts(Port startPort);

        [NotNull] private readonly FindCompatiblePorts _findCompatiblePorts;

        public GraphView([NotNull] FindCompatiblePorts findCompatiblePorts)
        {
            _findCompatiblePorts = findCompatiblePorts;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return _findCompatiblePorts(startPort).ToList();
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
            miniMap.windowed = true;
            miniMap.graphView = graphView;
            miniMap.name = "minimap";
        }
    }
}