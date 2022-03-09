#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeCreationMenuEntry<TNode> : IMenuEntry where TNode : INode<GraphRuntime<TNode>>
    {
        protected readonly GraphRuntime<TNode> _GraphRuntime;

        public NodeCreationMenuEntry(GraphRuntime<TNode> graphRuntime)
        {
            _GraphRuntime = graphRuntime;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            var memoryNodes = TypeCache.GetTypesDerivedFrom<TNode>();
            foreach (var nodeType in memoryNodes
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .OrderBy(type => type.Name))
            {
                menu.AddItem(new GUIContent($"{typeof(TNode).Name}/{nodeType.Name}"), false, () => CreateNode(nodeType, menuPosition));
            }
        }

        protected virtual NodeId CreateNode(Type nodeType, Vector2 position)
        {
            var node = (TNode)Activator.CreateInstance(nodeType);
            var nodeId = Guid.NewGuid();
            _GraphRuntime.AddNode(nodeId, node);
            return nodeId;
        }
    }
}

#endif