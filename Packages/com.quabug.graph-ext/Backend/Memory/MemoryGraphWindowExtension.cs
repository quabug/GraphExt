using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class MemoryGraphWindowExtension : IGraphWindowExtension
    {
        public void RecreateGraphView(VisualElement root)
        {
            var graph = root.Q<UnityEditor.Experimental.GraphView.GraphView>();
            graph?.parent.Remove(graph);
        }

        public void Tick()
        {
        }

        public void Clear()
        {
        }
    }
}