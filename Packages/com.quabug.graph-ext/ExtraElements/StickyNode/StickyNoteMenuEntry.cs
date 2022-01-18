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
                    if (graph.selection != null)
                    {
                        foreach (var note in graph.selection.OfType<StickyNote>())
                        {
                            _deleteNote(note);
                        }
                    }
                });
            }
        }
    }

    public class StickyNoteCreationMenuEntry : IMenuEntry
    {
        [NotNull] private readonly Action<NodeId, StickyNoteData> _addNote;

        public StickyNoteCreationMenuEntry([NotNull] Action<NodeId, StickyNoteData> addNote)
        {
            _addNote = addNote;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            menu.AddItem(new GUIContent("Create Note"), false, CreateNode);

            void CreateNode()
            {
                var nodeId = Guid.NewGuid();
                var data = new StickyNoteData
                {
                    X = menuPosition.x,
                    Y = menuPosition.y,
                    Width = 200,
                    Height = 200,
                    Title = "Note",
                    Content = "",
                    Theme = StickyNoteTheme.Dark,
                    FontSize = 0
                };
                _addNote(nodeId, data);
            }
        }
    }
}

#endif