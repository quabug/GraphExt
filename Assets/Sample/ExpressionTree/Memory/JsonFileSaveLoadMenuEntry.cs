using System;
using GraphExt.Editor;
using OneShot;
using UnityEngine;

public class JsonFileSaveLoadMenuEntry : IMenuEntryInstaller
{
    public TextAsset JsonFile;

    public void Install(Container container)
    {
        var child = container.CreateChildContainer();
        child.RegisterInstance<Action<TextAsset>>(json => JsonFile = json);
        child.RegisterSingleton<MemorySaveLoadMenu<IVisualNode>>();
        container.Register<IMenuEntry>(() => child.Resolve<MemorySaveLoadMenu<IVisualNode>>());
        if (JsonFile != null)
        {
            var (graphRuntime, nodePositions, notes) = JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
            container.RegisterGraphRuntimeInstance(graphRuntime);
            container.RegisterDictionaryInstance(nodePositions);
            container.RegisterDictionaryInstance(notes);
        }
    }
}