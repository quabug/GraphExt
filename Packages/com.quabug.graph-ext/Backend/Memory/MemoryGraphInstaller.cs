using System.Collections.Generic;
using OneShot;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryGraphInstaller<TNode> : IGraphInstaller where TNode : INode<GraphRuntime<TNode>>
    {
        public void Install(Container container, TypeContainers _)
        {
            container.RegisterGraphRuntimeInstance(new GraphRuntime<TNode>());
            container.RegisterDictionaryInstance(new Dictionary<NodeId, Vector2>());
        }
    }
}