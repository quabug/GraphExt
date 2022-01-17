#if UNITY_EDITOR

using System;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class StickyNodePresenter : IViewPresenter
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
        [NotNull] private readonly Func<IEnumerable<NodeId>> _stickyNotes;
        [NotNull] private readonly IBiDictionary<NodeId, StickyNote> _stickyNoteViews;
        [NotNull] private readonly Func<NodeId, StickyNoteData> _getStickyNoteData;
        [NotNull] private readonly Action<NodeId, StickyNoteData> _setStickyNoteData;

        public StickyNodePresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView graphView,
            [NotNull] Func<IEnumerable<NodeId>> stickyNotes,
            [NotNull] IBiDictionary<NodeId, StickyNote> stickyNoteViews,
            [NotNull] Func<NodeId, StickyNoteData> getStickyNoteData,
            [NotNull] Action<NodeId, StickyNoteData> setStickyNoteData
        )
        {
            _graphView = graphView;
            _stickyNotes = stickyNotes;
            _stickyNoteViews = stickyNoteViews;
            _getStickyNoteData = getStickyNoteData;
            _setStickyNoteData = setStickyNoteData;
        }

        public void Tick()
        {
            var (added, removed) = _stickyNoteViews.Keys.Diff(_stickyNotes());

            foreach (var note in added)
            {
                var view = new StickyNote();
                var data = _getStickyNoteData(note);
                _stickyNoteViews.Add(note, view);
                _graphView.AddElement(view);
                view.SetPosition(data.Rect);
                view.title = data.Title;
                view.contents = data.Content;
                view.theme = data.Theme.ToEditor();
                view.fontSize = data.FontSize.ToEditor();
                view.RegisterCallback<StickyNoteChangeEvent>(OnStickyNoteChanged);
            }

            foreach (var note in removed)
            {
                var view = _stickyNoteViews[note];
                view.UnregisterCallback<StickyNoteChangeEvent>(OnStickyNoteChanged);
                _graphView.RemoveElement(view);
                _stickyNoteViews.Remove(note);
            }
        }

        private void OnStickyNoteChanged(StickyNoteChangeEvent evt)
        {
            var view = (StickyNote)evt.target;
            var nodeId = _stickyNoteViews.GetKey(view);
            var data = new StickyNoteData()
            {
                Rect = view.GetPosition(),
                Title = view.title,
                Content = view.contents,
                Theme = view.theme.ToRuntime(),
                FontSize = view.fontSize.ToRuntime()
            };
            _setStickyNoteData(nodeId, data);
        }
    }
}

#endif