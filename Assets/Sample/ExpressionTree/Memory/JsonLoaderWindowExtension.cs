#if UNITY_EDITOR

using System;
using GraphExt.Editor;
using UnityEngine;

[Serializable]
public class JsonLoaderWindowExtension : IWindowExtension
{
    public TextAsset JsonFile;

    public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
    {
        view.Module = JsonFile == null ? new MemoryGraphViewModule<IVisualNode>() : JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
    }

    public void OnClosed(GraphWindow window, GraphConfig config, GraphView view) {}
    public IWindowExtension CreateNew()
    {
        return new JsonLoaderWindowExtension { JsonFile = JsonFile };
    }
}

#endif