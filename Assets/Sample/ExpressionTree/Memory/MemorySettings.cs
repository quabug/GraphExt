#if UNITY_EDITOR

using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using UnityEditor.Experimental.GraphView;

public class VisualMemoryGraphInstaller : MemoryGraphInstaller<IVisualNode> {}

public class VisualMemoryCreationMenuEntry : MemoryNodeCreationMenuEntry<IVisualNode>
{
    public VisualMemoryCreationMenuEntry(GraphRuntime<IVisualNode> graphRuntime, IReadOnlyDictionary<NodeId, Node> nodes) : base(graphRuntime, nodes)
    {
    }
}

#endif