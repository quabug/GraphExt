#if UNITY_EDITOR

using GraphExt.Editor;

public class JsonLoaderWindowExtension : IWindowExtension
{
    public string FilePath;

    public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
    {
        view.Module = JsonSaveLoad.Load(FilePath);
    }

    public void OnClosed(GraphWindow window, GraphConfig config, GraphView view) {}
}

#endif