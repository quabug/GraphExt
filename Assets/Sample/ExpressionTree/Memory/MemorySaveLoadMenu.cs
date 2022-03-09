#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

public class MemorySaveLoadMenu<TNode> : IMenuEntry where TNode : INode<GraphRuntime<TNode>>
{
    [NotNull] private readonly GraphRuntime<TNode> _graphRuntime;
    [NotNull] private readonly IReadOnlyDictionary<NodeId, Node> _nodes;
    [NotNull] private readonly IReadOnlyDictionary<StickyNoteId, StickyNoteData> _notes;
    [NotNull] private readonly Action<TextAsset> _setJsonFile;

    public MemorySaveLoadMenu(
        [NotNull] GraphRuntime<TNode> graphRuntime,
        [NotNull] IReadOnlyDictionary<NodeId, Node> nodes,
        [NotNull] IReadOnlyDictionary<StickyNoteId, StickyNoteData> notes,
        [NotNull] Action<TextAsset> setJsonFile
    )
    {
        _graphRuntime = graphRuntime;
        _nodes = nodes;
        _notes = notes;
        _setJsonFile = setJsonFile;
    }

    public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Save"), false, () =>
        {
            ClosePopupWindow();
            var path = EditorUtility.SaveFilePanel("save path", Application.dataPath, "graph", "json");
            var jsonAssetPath = ToRelativePath(path);
            var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonAssetPath);
            if (jsonAsset == null)
            {
                jsonAsset = new TextAsset();
                AssetDatabase.CreateAsset(jsonAsset, jsonAssetPath);
            }
            File.WriteAllText(path, JsonSaveLoad.Serialize(
                new JsonUtility.GraphRuntimeData<TNode>(_graphRuntime),
                new JsonEditorUtility.GraphViewData<TNode>(_nodes.ToDictionary(node => node.Key, node => node.Value.GetPosition().position)),
                _notes
            ));
            _setJsonFile(jsonAsset);
        });

        menu.AddItem(new GUIContent("Load"), false, () =>
        {
            ClosePopupWindow();

            var path = EditorUtility.OpenFilePanel("load path", Application.dataPath, "json");
            var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(ToRelativePath(path));
            _setJsonFile(jsonAsset);
            GetWindow()?.CreateGUI();
        });

        menu.AddItem(new GUIContent("Reset"), false, () =>
        {
            ClosePopupWindow();
            GetWindow()?.CreateGUI();
        });

        menu.AddItem(new GUIContent("ClearGraph"), false, () =>
        {
            ClosePopupWindow();
            ClearGraph();
        });

        void ClearGraph()
        {
            foreach (var node in _graphRuntime.NodeMap.Keys.ToArray()) _graphRuntime.DeleteNode(node);
        }
    }

    string ToRelativePath([PathReference] string absolutePath) => "Assets" + absolutePath.Substring(Application.dataPath.Length);

    void ClosePopupWindow()
    {
        var window = EditorWindow.focusedWindow as PopupWindow;
        if (window != null) window.Close();
    }

    GraphWindow GetWindow() => EditorWindow.focusedWindow as GraphWindow;
}

#endif