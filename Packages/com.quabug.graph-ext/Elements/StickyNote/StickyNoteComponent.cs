using System;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public class StickyNoteComponent : MonoBehaviour
    {
        [SerializeField, HideInInspector] private string _id;
        public StickyNoteId Id
        {
            get =>Guid.Parse(_id);
            set => _id = value.ToString();
        }
        public StickyNoteData Data;

        public void Init([NotNull] GameObject graphRoot, in StickyNoteId id, StickyNoteData data)
        {
            hideFlags = HideFlags.None;
            Id = id;
            Data = data;
            name = "Note";
            transform.SetParent(graphRoot.transform);
        }
    }
}