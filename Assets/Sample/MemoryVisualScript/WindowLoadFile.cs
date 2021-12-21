#if UNITY_EDITOR

using GraphExt.Editor;

public class WindowLoadFile : IWindowExtension
{
    public string FilePath;

    public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
    {
        if (string.IsNullOrEmpty(FilePath)) return;
        var graph = JsonSaveLoad.Load(FilePath);
        if (graph != null) view.Module = graph;
        else FilePath = null;
    }
}

#endif