#if UNITY_EDITOR

using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class StickyNodePresenter : IViewPresenter
    {
        public delegate void SetData(in StickyNoteId id, StickyNoteData data);

        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
        [NotNull] private readonly IStickyNoteViewFactory _viewFactory;
        [NotNull] private readonly IBiDictionary<StickyNoteId, StickyNote> _stickyNoteViews;
        [NotNull] private readonly SetData _setStickyNoteData;

        public StickyNodePresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView graphView,
            [NotNull] IStickyNoteViewFactory viewFactory,
            [NotNull] IBiDictionary<StickyNoteId, StickyNote> stickyNoteViews,
            [NotNull] SetData setStickyNoteData
        )
        {
            _graphView = graphView;
            _viewFactory = viewFactory;
            _stickyNoteViews = stickyNoteViews;
            _setStickyNoteData = setStickyNoteData;
        }

        public void CreateNoteView(in StickyNoteId id, StickyNoteData data)
        {
            var view = _viewFactory.Create(data);
            _stickyNoteViews.Add(id, view);
            _graphView.AddElement(view);
            view.RegisterCallback<StickyNoteChangeEvent>(OnStickyNoteChanged);
        }

        public void RemoveNoteView(in StickyNoteId id)
        {
            var view = _stickyNoteViews[id];
            view.UnregisterCallback<StickyNoteChangeEvent>(OnStickyNoteChanged);
            _graphView.RemoveElement(view);
            _stickyNoteViews.Remove(id);
        }

        private void OnStickyNoteChanged(StickyNoteChangeEvent evt)
        {
            var view = (StickyNote)evt.target;
            var noteId = _stickyNoteViews.GetKey(view);
            var rect = view.GetPosition();
            var data = new StickyNoteData
            (
                x: rect.x,
                y: rect.y,
                width: rect.width,
                height: rect.height,
                title: view.title,
                content: view.contents,
                theme: view.theme.ToRuntime(),
                fontSize: view.fontSize.ToRuntime()
            );
            _setStickyNoteData(noteId, data);
        }
    }
}

#endif