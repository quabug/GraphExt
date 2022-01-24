﻿#if UNITY_EDITOR

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class PrefabStickyNoteSystem : StickyNoteSystem
    {
        [NotNull] private readonly GameObject _root;

        private readonly Dictionary<StickyNoteId, StickyNoteComponent> _notes =
            new Dictionary<StickyNoteId, StickyNoteComponent>();

        public IReadOnlyDictionary<StickyNoteId, StickyNoteComponent> StickyNotes => _notes;

        public PrefabStickyNoteSystem(
            [NotNull] StickyNotePresenter presenter,
            [NotNull] BiDictionary<StickyNoteId, StickyNote> views,
            [NotNull] GameObject root
        ) : base(presenter, views)
        {
            _root = root;
            foreach (var note in root.GetComponentsInChildren<StickyNoteComponent>())
            {
                _notes.Add(note.Id, note);
                stickyNotePresenter.CreateNoteView(note.Id, note.Data);
            }
        }

        protected override void SetNodeData(in StickyNoteId id, StickyNoteData data)
        {
            _notes[id].Data = data;
            SavePrefabScene();
        }

        protected override void AddNoteData(in StickyNoteId id, StickyNoteData data)
        {
            var noteObject = new GameObject("Note");
            var noteInstance = noteObject.AddComponent<StickyNoteComponent>();
            noteInstance.Init(_root, id, data);
            _notes[id] = noteInstance;
            SavePrefabScene();
        }

        protected override void RemoveNoteData(in StickyNoteId id)
        {
            var noteInstance = _notes[id];
            _notes.Remove(id);
            Object.DestroyImmediate(noteInstance.gameObject);
            SavePrefabScene();
        }

        private void SavePrefabScene()
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(_root.scene);
        }
    }
}

#endif