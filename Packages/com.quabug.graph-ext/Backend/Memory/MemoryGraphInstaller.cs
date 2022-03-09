#if UNITY_EDITOR

namespace GraphExt.Editor
{
    public class MemoryGraphInstaller<TNode> : IGraphInstaller where TNode : INode<GraphRuntime<TNode>>
    {
        public void Install(Container container, TypeContainers _)
        {
            container.RegisterGraphRuntimeInstance(new GraphRuntime<TNode>());
        }
    }
}

#endif