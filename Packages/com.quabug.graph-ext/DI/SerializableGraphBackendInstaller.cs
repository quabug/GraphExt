#if UNITY_EDITOR

using System;
using Object = UnityEngine.Object;

namespace GraphExt.Editor
{
    [Serializable]
    public class SerializableGraphBackendInstaller<TNode, TNodeComponent> : IGraphInstaller
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeComponent: Object
    {
        public virtual void Install(Container container, TypeContainers typeContainers)
        {
            OverrideConvertToNodeData();
            RegisterSyncSelectionGraphElementPresenter();

            void OverrideConvertToNodeData()
            {
                typeContainers.GetTypeContainer(typeof(NodeViewPresenter)).Register<ConvertToNodeData>((resolveContainer, contractType) => {
                    var graph = container.Resolve<ISerializableGraphBackend<TNode, TNodeComponent>>();
                    return NodeDataConvertor.ToNodeData(id => graph.NodeMap[id], id => graph.SerializedObjects[id]);
                }).AsSelf();
            }

            void RegisterSyncSelectionGraphElementPresenter()
            {
                var presenterContainer = typeContainers.CreateSystemContainer(container, typeof(SyncSelectionGraphElementPresenter));
                presenterContainer.Register<Func<SyncSelectionGraphElementPresenter.IConvertor, SyncSelectionGraphElementPresenter.System>>(
                    (resolveContainer, contractType) =>
                    {
                        var graphView = container.Resolve<UnityEditor.Experimental.GraphView.GraphView>();
                        return convertor => new SyncSelectionGraphElementPresenter.System(graphView, convertor);
                    }).AsSelf();
            }
        }
    }
}

#endif