#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using UnityEngine;

public class JsonFileSaveLoadMenuEntry : IMenuEntryInstaller
{
    public TextAsset JsonFile;

    public void Install(Container container)
    {
        container.RegisterInstance<Action<TextAsset>>(json => JsonFile = json).AsSelf();
        container.Register<MemorySaveLoadMenu<IVisualNode>>().As<IMenuEntry>();
        if (JsonFile != null)
        {
            var (graphRuntime, nodePositions, notes) = JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
            container.RegisterGraphRuntimeInstance(graphRuntime);
            container.RegisterInstance(nodePositions).As<IReadOnlyDictionary<NodeId, Vector2>>();
            container.RegisterDictionaryInstance(notes);
        }
    }
}

#endif