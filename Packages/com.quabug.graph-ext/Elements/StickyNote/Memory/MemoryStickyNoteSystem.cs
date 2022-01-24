﻿#if UNITY_EDITOR

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class MemoryStickyNoteSystem : StickyNoteSystem
    {
        private readonly Dictionary<StickyNoteId, StickyNoteData> _stickyNotes = new Dictionary<StickyNoteId, StickyNoteData>();
        public IReadOnlyDictionary<StickyNoteId, StickyNoteData> StickyNotes => _stickyNotes;

        public MemoryStickyNoteSystem(
            [NotNull] StickyNotePresenter presenter,
            [NotNull] BiDictionary<StickyNoteId, StickyNote> views,
            [NotNull] IReadOnlyDictionary<StickyNoteId, StickyNoteData> notes
        ) : base(presenter, views)
        {
            foreach (var notePair in notes)
            {
                _stickyNotes.Add(notePair.Key, notePair.Value);
                StickyNotePresenter.CreateNoteView(notePair.Key, notePair.Value);
            }
        }

        protected override void SetNodeData(in StickyNoteId id, StickyNoteData data)
        {
            _stickyNotes[id] = data;
        }

        protected override void AddNoteData(in StickyNoteId id, StickyNoteData data)
        {
            _stickyNotes.Add(id, data);
        }

        protected override void RemoveNoteData(in StickyNoteId id)
        {
            _stickyNotes.Remove(id);
        }
    }
}

#endif