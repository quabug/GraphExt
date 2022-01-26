#if UNITY_EDITOR

using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class StickyNoteDeletionMenuEntry : IMenuEntry
    {
        [NotNull] private readonly RemoveNoteView _deleteNoteView;

        public StickyNoteDeletionMenuEntry([NotNull] RemoveNoteView deleteNoteView)
        {
            _deleteNoteView = deleteNoteView;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            if (graph.selection != null && graph.selection.Any(selection => selection is StickyNote))
            {
                menu.AddItem(new GUIContent("Delete Note"), false, () =>
                {
                    var count = graph.selection.Count;
                    for (var i = count - 1; i >= 0; i--)
                    {
                        if (graph.selection[i] is StickyNote note)
                            _deleteNoteView(note);
                    }
                });
            }
        }
    }
}

#endif