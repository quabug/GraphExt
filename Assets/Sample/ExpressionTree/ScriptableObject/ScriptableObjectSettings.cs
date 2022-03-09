#if UNITY_EDITOR

using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;

public class VisualScriptableObjectGraphWindowExtension : ScriptableObjectGraphWindowExtension<IVisualNode, VisualNodeScriptableObject> {}
public class VisualScriptableObjectInstaller : ScriptableObjectGraphBackendInstaller<IVisualNode, VisualNodeScriptableObject> {}

public class VisualScriptableObjectNodeCreationMenuEntry : ScriptableObjectNodeCreationMenuEntry<IVisualNode, VisualNodeScriptableObject>
{
    public VisualScriptableObjectNodeCreationMenuEntry(GraphRuntime<IVisualNode> graphRuntime, IReadOnlyDictionary<NodeId, VisualNodeScriptableObject> nodes) : base(graphRuntime, nodes)
    {
    }
}

#endif