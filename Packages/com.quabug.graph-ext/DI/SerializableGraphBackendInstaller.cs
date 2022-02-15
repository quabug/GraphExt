#if UNITY_EDITOR

using System.Collections.Generic;
using OneShot;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class SerializableGraphBackendInstaller<TNode, TNodeComponent> : IGraphInstaller
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeComponent: Object
    {
        public void Install(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.GetTypeContainer(typeof(NodeViewPresenter));
            presenterContainer.RegisterSingleton<ConvertToNodeData>(() => {
                var graph = container.Resolve<ISerializableGraphBackend<TNode, TNodeComponent>>();
                return NodeDataConvertor.ToNodeData(id => graph.NodeMap[id], id => graph.SerializedObjects[id]);
            });

            container.RegisterSingleton<IWindowSystem>(() =>
            {
                var graphView = container.Resolve<UnityEditor.Experimental.GraphView.GraphView>();
                var nodeViews = container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                var nodes = container.Resolve<IReadOnlyDictionary<TNodeComponent, NodeId>>();
                return new FocusActiveNodePresenter<TNodeComponent>(
                    graphView,
                    node => nodeViews[nodes[node]],
                    () => Selection.activeObject as TNodeComponent
                );
            });

            container.RegisterSingleton<IWindowSystem>(() =>
            {
                var nodes = container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                var nodeObjects = container.Resolve<IReadOnlyDictionary<NodeId, TNodeComponent>>();
                return new ActiveSelectedNodePresenter<TNodeComponent>(nodes, nodeObjects, node =>
                {
                    if (Selection.activeObject != node) Selection.activeObject = node;
                });
            });
        }
    }
}

#endif