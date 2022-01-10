#if UNITY_EDITOR

using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeMenuEntry<TNode> : IMenuEntry where TNode : INode<GraphRuntime<TNode>>
    {
        [NotNull] private readonly GraphRuntime<TNode> _graphRuntime;
        [NotNull] private readonly IViewModuleElements<NodeId, Vector2> _positions;

        public NodeMenuEntry(
            [NotNull] GraphRuntime<TNode> graphRuntime,
            [NotNull] IViewModuleElements<NodeId, Vector2> positions
        )
        {
            _graphRuntime = graphRuntime;
            _positions = positions;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            var memoryNodes = TypeCache.GetTypesDerivedFrom<TNode>();
            foreach (var nodeType in memoryNodes
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .OrderBy(type => type.Name))
            {
                menu.AddItem(new GUIContent($"{typeof(TNode).Name}/{nodeType.Name}"), false, () => CreateNode(nodeType));
            }

            void CreateNode(Type nodeType)
            {
                var node = (TNode)Activator.CreateInstance(nodeType);
                var nodeId = Guid.NewGuid();
                _graphRuntime.AddNode(nodeId, node);
                _positions[nodeId] = menuPosition;
            }
        }
    }
}

#endif