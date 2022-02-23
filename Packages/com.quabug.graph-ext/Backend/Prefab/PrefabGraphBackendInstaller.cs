#if UNITY_EDITOR

using OneShot;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class PrefabGraphBackendInstaller<TNode, TNodeComponent> : SerializableGraphBackendInstaller<TNode, TNodeComponent>
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeComponent: Component
    {
        public override void Install(Container container, TypeContainers typeContainers)
        {
            base.Install(container, typeContainers);
            container.Register<IWindowSystem>(() =>
            {
                var graphView = container.Resolve<UnityEditor.Experimental.GraphView.GraphView>();
                var nodeViews = container.Resolve<IReadOnlyBiDictionary<NodeId, Node>>();
                var nodes = container.Resolve<IReadOnlyBiDictionary<NodeId, TNodeComponent>>();
                return new SyncSelectionGraphElementPresenter(
                    graphView,
                    selectable => selectable is Node node ? nodes[nodeViews.Reverse[node]].gameObject : null,
                    obj =>
                    {
                        var nodeComponent = obj is GameObject node ? node.GetComponent<TNodeComponent>() : null;
                        return nodeComponent == null ? null : nodeViews[nodes.Reverse[nodeComponent]];
                    });
            });
        }
    }
}

#endif
