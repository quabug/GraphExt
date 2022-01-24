#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class ScriptableObjectStickyNoteSystem : StickyNoteSystem
    {
        [NotNull] private readonly ScriptableObject _graph;

        private readonly Dictionary<StickyNoteId, StickyNoteScriptableObject> _notes =
            new Dictionary<StickyNoteId, StickyNoteScriptableObject>();

        public IReadOnlyDictionary<StickyNoteId, StickyNoteScriptableObject> StickyNotes => _notes;

        public ScriptableObjectStickyNoteSystem(
            [NotNull] StickyNotePresenter presenter,
            [NotNull] BiDictionary<StickyNoteId, StickyNote> views,
            [NotNull] ScriptableObject graph
        ) : base(presenter, views)
        {
            _graph = graph;
            var path = AssetDatabase.GetAssetPath(graph);
            foreach (var note in AssetDatabase.LoadAllAssetsAtPath(path).OfType<StickyNoteScriptableObject>())
            {
                _notes.Add(note.Id, note);
                stickyNotePresenter.CreateNoteView(note.Id, note.Data);
            }
        }

        protected override void SetNodeData(in StickyNoteId id, StickyNoteData data)
        {
            _notes[id].Data = data;
            Save();
        }

        protected override void AddNoteData(in StickyNoteId id, StickyNoteData data)
        {
            var noteObject = ScriptableObject.CreateInstance<StickyNoteScriptableObject>();
            noteObject.Init(_graph, id, data);
            _notes[id] = noteObject;
            Save();
        }

        protected override void RemoveNoteData(in StickyNoteId id)
        {
            var noteInstance = _notes[id];
            _notes.Remove(id);
            Object.DestroyImmediate(noteInstance, allowDestroyingAssets: true);
            Save();
        }

        private void Save()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

#endif