#if UNITY_EDITOR

using GraphExt.Editor;

public class VisualPrefabGraphWindowExtension : PrefabGraphWindowExtension<IVisualNode, VisualTreeComponent> {}
public class VisualPrefabInstaller : SerializableGraphBackendInstaller<IVisualNode, VisualTreeComponent> {}

#endif