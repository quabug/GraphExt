#if UNITY_EDITOR

using GraphExt.Editor;

public class VisualScriptableObjectGraphWindowExtension : ScriptableObjectGraphWindowExtension<IVisualNode, VisualNodeScriptableObject> {}
public class VisualScriptableObjectInstaller : SerializableGraphBackendInstaller<IVisualNode, VisualNodeScriptableObject> {}

#endif