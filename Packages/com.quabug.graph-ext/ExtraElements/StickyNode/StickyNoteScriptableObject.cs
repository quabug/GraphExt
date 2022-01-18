using System;
using UnityEngine;

namespace GraphExt
{
    public class StickyNoteScriptableObject : ScriptableObject
    {
        public NodeId NodeId
        {
            get =>Guid.Parse(Id);
            set => Id = value.ToString();
        }
        [HideInInspector] public string Id;
        public StickyNoteData Data;

        public void Init(ScriptableObject graph, in NodeId nodeId, StickyNoteData data)
        {
            hideFlags = HideFlags.None;
            NodeId = nodeId;
            Data = data;
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.AddObjectToAsset(this, graph);
            name = "Note";
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}