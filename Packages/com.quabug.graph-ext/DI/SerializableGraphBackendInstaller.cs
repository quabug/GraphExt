#if UNITY_EDITOR

using OneShot;
using Object = UnityEngine.Object;

namespace GraphExt.Editor
{
    public class SerializableGraphBackendInstaller<TNode, TNodeComponent> : IGraphInstaller
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeComponent: Object
    {
        public virtual void Install(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.GetTypeContainer(typeof(NodeViewPresenter));
            presenterContainer.RegisterSingleton<ConvertToNodeData>(() => {
                var graph = container.Resolve<ISerializableGraphBackend<TNode, TNodeComponent>>();
                return NodeDataConvertor.ToNodeData(id => graph.NodeMap[id], id => graph.SerializedObjects[id]);
            });
        }
    }
}

#endif