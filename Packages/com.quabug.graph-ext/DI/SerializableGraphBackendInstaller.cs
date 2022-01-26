﻿#if UNITY_EDITOR

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
                var serializedObjects = container.Resolve<IReadOnlyDictionary<NodeId, SerializedObject>>();
                var nodes = container.Resolve<IReadOnlyDictionary<NodeId, TNodeComponent>>();
                return NodeDataConvertor.ToNodeData(id => nodes[id], id => serializedObjects[id]);
            });

            container.RegisterSingleton<IWindowSystem>(() =>
            {
                var graphView = container.Resolve<GraphView>();
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