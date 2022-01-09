using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class SelectionEntry : IMenuEntry
    {
        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            if (graph.selection != null && graph.selection.Any())
            {
                menu.AddItem(new GUIContent("Delete"), false, () => graph.DeleteSelection());
                menu.AddSeparator("");
            }
        }
    }
}