#if UNITY_EDITOR

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class MemoryStickyNoteSystem : StickyNoteSystem<StickyNoteData>
    {
        [NotNull] private readonly IDictionary<StickyNoteId, StickyNoteData> _notes;
        private readonly BiDictionary<StickyNoteId, StickyNoteData> _stickyNotes = new BiDictionary<StickyNoteId, StickyNoteData>();
        public override IReadOnlyBiDictionary<StickyNoteId, StickyNoteData> StickyNotes => _stickyNotes;

        public MemoryStickyNoteSystem(
            [NotNull] StickyNotePresenter presenter,
            [NotNull] BiDictionary<StickyNoteId, StickyNote> views,
            [NotNull] IDictionary<StickyNoteId, StickyNoteData> notes
        ) : base(presenter, views)
        {
            _notes = notes;
            foreach (var notePair in notes)
            {
                _stickyNotes.Add(notePair.Key, notePair.Value);
                StickyNotePresenter.CreateNoteView(notePair.Key, notePair.Value);
            }
        }

        protected override void SetNodeData(in StickyNoteId id, StickyNoteData data)
        {
            _stickyNotes[id] = data;
            _notes[id] = data;
        }

        protected override void AddNoteData(in StickyNoteId id, StickyNoteData data)
        {
            _stickyNotes.Add(id, data);
            _notes.Add(id, data);
        }

        protected override void RemoveNoteData(in StickyNoteId id)
        {
            _stickyNotes.Remove(id);
            _notes.Remove(id);
        }
    }
}

#endif