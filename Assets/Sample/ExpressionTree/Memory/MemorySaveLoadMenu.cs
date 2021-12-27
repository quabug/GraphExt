#if UNITY_EDITOR

using System.IO;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

public class MemorySaveLoadMenu : IMenuEntry
{
    public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
    {
        if (graph.Module is MemoryGraphViewModule<IVisualNode> module)
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
                File.WriteAllText(path, module.Serialize());
                ChangeWindowFilePath(jsonAsset);
            });

            menu.AddItem(new GUIContent("Load"), false, () =>
            {
                ClosePopupWindow();

                var path = EditorUtility.OpenFilePanel("load path", Application.dataPath, "json");
                var json = File.ReadAllText(path);
                var newGraph = JsonEditorUtility.Deserialize<IVisualNode>(json);
                if (newGraph != null)
                {
                    graph.Module = newGraph;
                    var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(ToRelativePath(path));
                    ChangeWindowFilePath(jsonAsset);
                }
            });

            menu.AddItem(new GUIContent("Reset"), false, () =>
            {
                ClosePopupWindow();
                var jsonAsset = CurrentWindowJsonAsset();
                if (jsonAsset != null)
                {
                    var newGraph = JsonEditorUtility.Deserialize<IVisualNode>(jsonAsset.text);
                    if (newGraph != null) graph.Module = newGraph;
                }
            });

            menu.AddItem(new GUIContent("Clear"), false, () =>
            {
                ClosePopupWindow();
                graph.Module = new MemoryGraphViewModule<IVisualNode>();
            });
        }
    }

    string ToRelativePath([PathReference] string absolutePath) => "Assets" + absolutePath.Substring(Application.dataPath.Length);

    void ChangeWindowFilePath(TextAsset json)
    {
        var window = EditorWindow.focusedWindow as GraphWindow;
        if (window != null) window.WindowExtension.GetOrCreate<JsonLoaderWindowExtension>().JsonFile = json;
    }

    TextAsset CurrentWindowJsonAsset()
    {
        var window = EditorWindow.focusedWindow as GraphWindow;
        return window == null ? null : window.WindowExtension.GetOrCreate<JsonLoaderWindowExtension>().JsonFile;
    }

    void ClosePopupWindow()
    {
        var window = EditorWindow.focusedWindow as PopupWindow;
        if (window != null) window.Close();
    }
}

#endif