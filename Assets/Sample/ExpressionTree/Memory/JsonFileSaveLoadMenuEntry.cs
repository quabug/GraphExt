#if UNITY_EDITOR

using System;
using GraphExt;
using GraphExt.Editor;
using UnityEngine;

public class JsonFileSaveLoadMenuEntry : IMenuEntryInstaller
{
    public TextAsset JsonFile;

    public void Install(Container container)
    {
        var child = container.CreateChildContainer();
        child.RegisterInstance<Action<TextAsset>>(json => JsonFile = json).AsSelf();
        child.Register<MemorySaveLoadMenu<IVisualNode>>().As<IMenuEntry>();
        if (JsonFile != null)
        {
            var (graphRuntime, nodePositions, notes) = JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
            container.RegisterGraphRuntimeInstance(graphRuntime);
            container.RegisterDictionaryInstance(nodePositions);
            container.RegisterDictionaryInstance(notes);
        }
    }
}

#endif