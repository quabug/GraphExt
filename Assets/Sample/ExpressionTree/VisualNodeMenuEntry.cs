#if UNITY_EDITOR

using GraphExt;
using GraphExt.Editor;

public class VisualNodeMenuEntry : NodeMenuEntry<IVisualNode>
{
    public VisualNodeMenuEntry(GraphRuntime<IVisualNode> graphRuntime, InitializeNodePosition initializeNodePosition) : base(graphRuntime, initializeNodePosition)
    {
    }
}

#endif