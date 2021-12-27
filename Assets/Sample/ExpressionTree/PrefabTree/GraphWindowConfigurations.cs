#if UNITY_EDITOR

using GraphExt.Editor;
using JetBrains.Annotations;

public static class PrefabVisualTree
{
    [UsedImplicitly] private class NodeCreation : NodeCreationMenuEntry<IVisualNode, VisualTreeComponent> {}
    [UsedImplicitly] private class WindowExtension : PrefabStageWindowExtension<IVisualNode, VisualTreeComponent> {}
}

#endif