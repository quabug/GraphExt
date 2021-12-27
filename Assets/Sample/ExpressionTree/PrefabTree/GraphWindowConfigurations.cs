#if UNITY_EDITOR

using GraphExt.Prefab;

public static class PrefabVisualTree
{
    public class NodeCreation : NodeCreationMenuEntry<IVisualNode, VisualTreeComponent> {}
    public class WindowExtension : PrefabStageWindowExtension<IVisualNode, VisualTreeComponent> {}
}

#endif