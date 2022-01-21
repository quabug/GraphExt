#if UNITY_EDITOR

using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class StickyNoteCreationMenuEntry : IMenuEntry
    {
        public delegate void AddNote(in StickyNoteId id, StickyNoteData data);
        private readonly AddNote _addNote;

        public StickyNoteCreationMenuEntry([NotNull] AddNote addNote)
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