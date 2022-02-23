#if UNITY_EDITOR

using GraphExt;
using GraphExt.Editor;
using UnityEditor.Experimental.SceneManagement;

public class VisualPrefabGraphWindowExtension : PrefabGraphWindowExtension<IVisualNode, VisualTreeComponent> {}
public class VisualPrefabInstaller : SerializableGraphBackendInstaller<IVisualNode, VisualTreeComponent> {}
public class VisualPrefabIsPortCompatibleInstaller : PrefabIsPortCompatibleInstaller<IVisualNode, VisualTreeComponent> {}
public class VisualPrefabNodeObjectObserver : NodeObjectDeletedObserver<IVisualNode, VisualTreeComponent>
{
    public VisualPrefabNodeObjectObserver(PrefabStage prefabStage, GameObjectNodes<IVisualNode, VisualTreeComponent> nodes) : base(prefabStage, nodes)
    {
    }
}

#endif
