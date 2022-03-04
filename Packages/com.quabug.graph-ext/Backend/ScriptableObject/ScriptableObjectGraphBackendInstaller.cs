using UnityEditor.Experimental.GraphView;

#if UNITY_EDITOR

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphBackendInstaller<TNode, TNodeComponent> : SerializableGraphBackendInstaller<TNode, TNodeComponent>
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeComponent: NodeScriptableObject
    {
        public override void Install(Container container, TypeContainers typeContainers)
        {
            base.Install(container, typeContainers);
            var presenterContainer = typeContainers.GetTypeContainer(typeof(SyncSelectionGraphElementPresenter));
            presenterContainer.Register<ScriptableNodeSelectionConvertor<NodeId, Node, TNodeComponent>>().Singleton().AsSelf().AsInterfaces();
        }
    }
}

#endif
