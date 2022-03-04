#if UNITY_EDITOR

using System;
using GraphExt.Editor;

public class VisualPrefabGraphWindowExtension : PrefabGraphWindowExtension<IVisualNode, VisualTreeComponent> {}
[Serializable] public class VisualPrefabInstaller : PrefabGraphBackendInstaller<IVisualNode, VisualTreeComponent> {}

#endif
