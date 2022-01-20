using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public abstract class StickyNoteSystem
    {
        public BiDictionary<StickyNoteId, StickyNote> NoteViews { get; } = new BiDictionary<StickyNoteId, StickyNote>();
        public StickyNodePresenter StickyNodePresenter { get; }

        public StickyNoteSystem( [NotNull] UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            StickyNodePresenter = new StickyNodePresenter(
                graphView,
                NoteViews,
                SetNodeData
            );
        }

        public void AddNote(in StickyNoteId id, StickyNoteData data)
        {
            AddNoteData(id, data);
            StickyNodePresenter.CreateNoteView(id, data);
        }

        public void RemoveNote(StickyNote view)
        {
            var id = NoteViews.GetKey(view);
            StickyNodePresenter.RemoveNoteView(id);
            RemoveNoteData(id);
        }

        protected abstract void SetNodeData(in StickyNoteId id, StickyNoteData data);
        protected abstract void AddNoteData(in StickyNoteId id, StickyNoteData data);
        protected abstract void RemoveNoteData(in StickyNoteId id);
    }
}