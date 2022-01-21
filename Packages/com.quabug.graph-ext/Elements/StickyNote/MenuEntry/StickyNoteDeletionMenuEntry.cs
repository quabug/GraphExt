#if UNITY_EDITOR

using System;
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
        [NotNull] private readonly Action<StickyNote> _deleteNote;

        public StickyNoteDeletionMenuEntry([NotNull] Action<StickyNote> deleteNote)
        {
            _deleteNote = deleteNote;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            if (graph.selection != null && graph.selection.Any())
            {
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    var count = graph.selection.Count;
                    for (var i = count - 1; i >= 0; i--)
                    {
                        if (graph.selection[i] is StickyNote note)
                            _deleteNote(note);
                    }
                });
            }
        }
    }
}

#endif