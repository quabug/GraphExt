#if UNITY_EDITOR

using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public delegate void AddNote(in StickyNoteId id, StickyNoteData data);
    public delegate void RemoveNoteView(StickyNote view);

    public abstract class StickyNoteSystem<TStickyNoteData>
    {
        public BiDictionary<StickyNoteId, StickyNote> NoteViews { get; }
        public StickyNotePresenter StickyNotePresenter { get; }
        public abstract IReadOnlyBiDictionary<StickyNoteId, TStickyNoteData> StickyNotes { get; }

        public StickyNoteSystem(
            [NotNull] StickyNotePresenter presenter,
            [NotNull] BiDictionary<StickyNoteId, StickyNote> views
        )
        {
            StickyNotePresenter = presenter;
            NoteViews = views;
            presenter.OnDataChanged += SetNodeData;
        }

        public void AddNote(in StickyNoteId id, StickyNoteData data)
        {
            AddNoteData(id, data);
            StickyNotePresenter.CreateNoteView(id, data);
        }

        public void RemoveNote(StickyNote view)
        {
            var id = NoteViews.GetKey(view);
            StickyNotePresenter.RemoveNoteView(id);
            RemoveNoteData(id);
        }

        protected abstract void SetNodeData(in StickyNoteId id, StickyNoteData data);
        protected abstract void AddNoteData(in StickyNoteId id, StickyNoteData data);
        protected abstract void RemoveNoteData(in StickyNoteId id);
    }
}

#endif