using System;
using UnityEngine;

namespace GraphExt
{
    public class StickyNoteScriptableObject : ScriptableObject
    {
        public StickyNoteId Id
        {
            get => Guid.Parse(_id);
            set => _id = value.ToString();
        }
        [SerializeField, HideInInspector] private string _id;
        public StickyNoteData Data;

        public void Init(ScriptableObject graph, in StickyNoteId id, StickyNoteData data)
        {
            hideFlags = HideFlags.None;
            Id = id;
            Data = data;
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.AddObjectToAsset(this, graph);
            name = "Note";
#endif
        }
    }
}