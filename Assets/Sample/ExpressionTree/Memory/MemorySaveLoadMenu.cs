// #if UNITY_EDITOR
//
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using GraphExt;
// using GraphExt.Editor;
// using JetBrains.Annotations;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UIElements;
// using PopupWindow = UnityEditor.PopupWindow;
//
// public class MemorySaveLoadMenu<TNode> : IMenuEntry where TNode : INode<GraphRuntime<TNode>>
// {
//     [NotNull] private readonly GraphRuntime<TNode> _graphRuntime;
//     [NotNull] private readonly IReadOnlyDictionary<NodeId, Vector2> _nodePositions;
//     [NotNull] private readonly IReadOnlyDictionary<StickyNoteId, StickyNoteData> _notes;
//
//     public MemorySaveLoadMenu(
//         [NotNull] GraphRuntime<TNode> graphRuntime,
//         [NotNull] IReadOnlyDictionary<NodeId, Vector2> nodePositions,
//         [NotNull] IReadOnlyDictionary<StickyNoteId, StickyNoteData> notes
//     )
//     {
//         _graphRuntime = graphRuntime;
//         _nodePositions = nodePositions;
//         _notes = notes;
//     }
//
//     public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
//     {
//         menu.AddItem(new GUIContent("Save"), false, () =>
//         {
//             ClosePopupWindow();
//             var path = EditorUtility.SaveFilePanel("save path", Application.dataPath, "graph", "json");
//             var jsonAssetPath = ToRelativePath(path);
//             var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonAssetPath);
//             if (jsonAsset == null)
//             {
//                 jsonAsset = new TextAsset();
//                 AssetDatabase.CreateAsset(jsonAsset, jsonAssetPath);
//             }
//             File.WriteAllText(path, JsonSaveLoad.Serialize(
//                 new JsonUtility.GraphRuntimeData<TNode>(_graphRuntime),
//                 new JsonEditorUtility.GraphViewData<TNode>(_nodePositions),
//                 _notes
//             ));
//             ChangeWindowFilePath(jsonAsset);
//         });
//
//         menu.AddItem(new GUIContent("Load"), false, () =>
//         {
//             ClosePopupWindow();
//
//             var path = EditorUtility.OpenFilePanel("load path", Application.dataPath, "json");
//             var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(ToRelativePath(path));
//             ChangeWindowFilePath(jsonAsset);
//             GetWindow()?.Recreate();
//         });
//
//         menu.AddItem(new GUIContent("Reset"), false, () =>
//         {
//             ClosePopupWindow();
//             GetWindow()?.Recreate();
//         });
//
//         menu.AddItem(new GUIContent("ClearGraph"), false, () =>
//         {
//             ClosePopupWindow();
//             ClearGraph();
//         });
//
//         void ClearGraph()
//         {
//             foreach (var node in _graphRuntime.NodeMap.Keys.ToArray()) _graphRuntime.DeleteNode(node);
//         }
//     }
//
//     string ToRelativePath([PathReference] string absolutePath) => "Assets" + absolutePath.Substring(Application.dataPath.Length);
//
//     void ChangeWindowFilePath(TextAsset json)
//     {
//         var window = GetWindow();
//         if (window != null) window.JsonFile = json;
//     }
//
//     TextAsset CurrentWindowJsonAsset()
//     {
//         var window = GetWindow();
//         return window == null ? null : window.JsonFile;
//     }
//
//     void ClosePopupWindow()
//     {
//         var window = EditorWindow.focusedWindow as PopupWindow;
//         if (window != null) window.Close();
//     }
//
//     MemoryExpressionTreeWindow GetWindow() => EditorWindow.focusedWindow as MemoryExpressionTreeWindow;
// }
//
// #endif