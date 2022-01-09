#if UNITY_EDITOR

using GraphExt.Editor;
using JetBrains.Annotations;

public static class PrefabVisualTree
{
    [UsedImplicitly] public class NodeCreation : NodeCreationMenuEntry<IVisualNode, VisualTreeComponent> {}
    // [UsedImplicitly] public class WindowExtension : PrefabStageWindowExtension<IVisualNode, VisualTreeComponent> {}
}

#endif