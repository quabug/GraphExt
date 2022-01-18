using System;
using GraphExt;
using JetBrains.Annotations;
using UnityEngine;

namespace ExtraElements.StickyNode
{
    public class StickyNoteComponent : MonoBehaviour
    {
        [SerializeField, HideInInspector] private string _id;
        public NodeId NodeId
        {
            get =>Guid.Parse(_id);
            set => _id = value.ToString();
        }
        public StickyNoteData Data;

        public void Init([NotNull] GameObject graphRoot, in NodeId nodeId, StickyNoteData data)
        {
            hideFlags = HideFlags.None;
            NodeId = nodeId;
            Data = data;
#if UNITY_EDITOR
            name = "Note";
            transform.SetParent(graphRoot.transform);
#endif
        }
    }
}