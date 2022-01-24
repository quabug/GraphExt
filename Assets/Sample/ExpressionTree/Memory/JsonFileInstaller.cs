using GraphExt.Editor;
using OneShot;
using UnityEngine;

public class JsonFileInstaller : IInstaller
{
    public TextAsset JsonFile;

    public void Install(Container container)
    {
        container.RegisterInstance(this);
        if (JsonFile != null)
        {
            var (graphRuntime, nodePositions, notes) = JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
            container.RegisterGraphRuntimeInstance(graphRuntime);
            container.RegisterDictionaryInstance(nodePositions);
            container.RegisterDictionaryInstance(notes);
        }
    }
}