#if UNITY_EDITOR

using GraphExt.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

public class MemorySaveLoadMenu : IMenuEntry
{
    public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
    {
        if (graph.Module is MemoryGraphViewModule module)
        {
            menu.AddItem(new GUIContent("Save"), false, () =>
            {
                ClosePopupWindow();
                var path = EditorUtility.SaveFilePanel("save path", Application.dataPath, "graph", "json");
                if (JsonEditorUtility.Save(module, path)) ChangeWindowFilePath(path);
            });
            menu.AddItem(new GUIContent("Load"), false, () =>
            {
                ClosePopupWindow();
                var path = EditorUtility.OpenFilePanel("load path", Application.dataPath, "json");
                var newGraph = JsonEditorUtility.Load(path);
                if (newGraph != null)
                {
                    graph.Module = newGraph;
                    ChangeWindowFilePath(path);
                }
            });

            menu.AddItem(new GUIContent("Reset"), false, () =>
            {
                ClosePopupWindow();
                var path = CurrentFilePath();
                if (!string.IsNullOrEmpty(path))
                {
                    var newGraph = JsonEditorUtility.Load(path);
                    if (newGraph != null) graph.Module = newGraph;
                }
            });

            menu.AddItem(new GUIContent("Clear"), false, () =>
            {
                ClosePopupWindow();
                graph.Module = new MemoryGraphViewModule();
            });
        }
    }

    void ChangeWindowFilePath(string path)
    {
        var window = EditorWindow.focusedWindow as GraphWindow;
        if (window != null) window.WindowExtension.GetOrCreate<JsonLoaderWindowExtension>().FilePath = path;
    }

    string CurrentFilePath()
    {
        var window = EditorWindow.focusedWindow as GraphWindow;
        return window?.WindowExtension.GetOrCreate<JsonLoaderWindowExtension>().FilePath;
    }

    void ClosePopupWindow()
    {
        var window = EditorWindow.focusedWindow as PopupWindow;
        window?.Close();
    }
}

#endif