#if UNITY_EDITOR

using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public delegate void AddNote(in StickyNoteId id, StickyNoteData data);
    public delegate void RemoveNoteView(StickyNote view);

    public abstract class StickyNoteSystem
    {
        public BiDictionary<StickyNoteId, StickyNote> NoteViews { get; }
        public StickyNotePresenter stickyNotePresenter { get; }

        public StickyNoteSystem(
            [NotNull] StickyNotePresenter presenter,
            [NotNull] BiDictionary<StickyNoteId, StickyNote> views
        )
        {
            NoteViews = views;
            presenter.OnDataChanged += SetNodeData;
        }

        public void AddNote(in StickyNoteId id, StickyNoteData data)
        {
            AddNoteData(id, data);
            stickyNotePresenter.CreateNoteView(id, data);
        }

        public void RemoveNote(StickyNote view)
        {
            var id = NoteViews.GetKey(view);
            stickyNotePresenter.RemoveNoteView(id);
            RemoveNoteData(id);
        }

        protected abstract void SetNodeData(in StickyNoteId id, StickyNoteData data);
        protected abstract void AddNoteData(in StickyNoteId id, StickyNoteData data);
        protected abstract void RemoveNoteData(in StickyNoteId id);
    }
}

#endif