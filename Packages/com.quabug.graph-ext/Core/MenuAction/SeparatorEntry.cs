using UnityEditor;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class SeparatorEntry : IMenuEntry
    {
        public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            menu.AddSeparator("");
        }
    }
}