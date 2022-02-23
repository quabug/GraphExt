#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using OneShot;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphBackendInstaller<TNode, TNodeComponent> : SerializableGraphBackendInstaller<TNode, TNodeComponent>
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeComponent: Object
    {
        public override void Install(Container container, TypeContainers typeContainers)
        {
            base.Install(container, typeContainers);
            var presenterContainer = typeContainers.CreateSystemContainer(container, typeof(SyncSelectionGraphElementPresenter));
            presenterContainer.Register<Func<IEnumerable<Node>>>(() =>
            {
                var nodeViews = container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                var nodes = container.Resolve<IReadOnlyDictionary<TNodeComponent, NodeId>>();
                return () => Selection.objects.Select(obj => obj is TNodeComponent nodeComponent ? nodeViews[nodes[nodeComponent]] : null);
            });
            presenterContainer.Register<Action<IEnumerable<Node>>>(() =>
            {
                var nodeViews = container.Resolve<IReadOnlyDictionary<Node, NodeId>>();
                var nodes = container.Resolve<IReadOnlyDictionary<NodeId, TNodeComponent>>();
                var graph = container.Resolve<ScriptableObject>();
                return selectedNodes =>
                {
                    var nodeObjects = selectedNodes.Select(nodeView => nodes[nodeViews[nodeView]]).OfType<Object>();
                    Selection.objects = nodeObjects.Any() ? nodeObjects.ToArray() : new Object[] {graph};
                };
            });
        }
    }
}

#endif
