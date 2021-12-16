using GraphExt;
using GraphExt.Memory;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MemorySaveLoadMenu : IMenuEntry
{
    public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
    {
        if (graph.Module is Graph memoryGraph)
        {
            menu.AddItem(new GUIContent("Save"), false, () =>
            {
                var path = EditorUtility.SaveFilePanel("save path", Application.dataPath, "graph", "json");
                if (JsonSaveLoad.Save(memoryGraph, path)) ChangeWindowFilePath(path);
            });
            menu.AddItem(new GUIContent("Load"), false, () =>
            {
                var path = EditorUtility.OpenFilePanel("load path", Application.dataPath, "json");
                var newGraph = JsonSaveLoad.Load(path);
                if (newGraph != null)
                {
                    graph.Module = newGraph;
                    ChangeWindowFilePath(path);
                }
            });

            menu.AddItem(new GUIContent("Reset"), false, () =>
            {
                var path = CurrentFilePath();
                if (!string.IsNullOrEmpty(path))
                {
                    var newGraph = JsonSaveLoad.Load(path);
                    if (newGraph != null) graph.Module = newGraph;
                }
            });
        }
    }

    void ChangeWindowFilePath(string path)
    {
        var window = EditorWindow.focusedWindow as GraphWindow;
        if (window != null) window.WindowExtension.GetOrCreate<WindowLoadFile>().FilePath = path;
    }

    string CurrentFilePath()
    {
        var window = EditorWindow.focusedWindow as GraphWindow;
        return window?.WindowExtension.GetOrCreate<WindowLoadFile>().FilePath;
    }
}