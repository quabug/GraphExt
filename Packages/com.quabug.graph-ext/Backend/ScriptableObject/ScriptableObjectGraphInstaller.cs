#if UNITY_EDITOR

using System.Collections.Generic;
using OneShot;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphInstaller<TNode, TNodeScriptableObject> : IGraphInstaller
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        public void Install(Container container)
        {
            container.RegisterSingleton<ConvertToNodeData>(() => {
                var graph = container.Resolve<GraphScriptableObject<TNode, TNodeScriptableObject>>();
                return NodeDataConvertor.ToNodeData(id => graph.NodeObjectMap[id], id => graph.SerializedObjects[id]);
            });

            container.RegisterSingleton(() =>
            {
                var graph = container.Resolve<GraphScriptableObject<TNode, TNodeScriptableObject>>();
                return new NodePositions<TNodeScriptableObject>(
                    graph.NodeObjectMap,
                    node => node.Position,
                    (node, pos) => node.Position = pos
                );
            });
            container.Register<IDictionary<NodeId, Vector2>>(container.Resolve<NodePositions<TNodeScriptableObject>>);
            container.Register<IReadOnlyDictionary<NodeId, Vector2>>(container.Resolve<NodePositions<TNodeScriptableObject>>);

            container.RegisterSingleton<IWindowSystem>(() =>
            {
                var graphView = container.Resolve<GraphView>();
                var nodeViews = container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                var graph = container.Resolve<GraphScriptableObject<TNode, TNodeScriptableObject>>();
                return new FocusActiveNodePresenter<TNodeScriptableObject>(
                    graphView,
                    node => nodeViews[graph[node]],
                    () => Selection.activeObject as TNodeScriptableObject
                );
            });

            container.RegisterSingleton<IWindowSystem>(() =>
            {
                var nodes = container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                var nodeObjects = container.Resolve<IReadOnlyDictionary<NodeId, TNodeScriptableObject>>();
                return new ActiveSelectedNodePresenter<TNodeScriptableObject>(nodes, nodeObjects, node =>
                {
                    if (Selection.activeObject != node) Selection.activeObject = node;
                });
            });
        }
    }
}

#endif