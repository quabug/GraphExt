#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;

public class VisualPrefabGraphWindowExtension : PrefabGraphWindowExtension<IVisualNode, VisualTreeComponent> {}
[Serializable] public class VisualPrefabInstaller : PrefabGraphBackendInstaller<IVisualNode, VisualTreeComponent> {}

public class VisualPrefabNodeCreationMenuEntry : PrefabNodeCreationMenuEntry<IVisualNode, VisualTreeComponent>
{
    public VisualPrefabNodeCreationMenuEntry(GraphRuntime<IVisualNode> graphRuntime, IReadOnlyDictionary<NodeId, VisualTreeComponent> nodes) : base(graphRuntime, nodes)
    {
    }
}

#endif
