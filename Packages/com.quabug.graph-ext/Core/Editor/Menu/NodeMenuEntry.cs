#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public delegate void InitializeNodePosition(in NodeId nodeId, Vector2 nodePosition);

    public class NodeMenuEntry<TNode> : IMenuEntry where TNode : INode<GraphRuntime<TNode>>
    {
        private readonly GraphRuntime<TNode> _graphRuntime;
        private readonly InitializeNodePosition _initializeNodePosition;

        public NodeMenuEntry(GraphRuntime<TNode> graphRuntime, InitializeNodePosition initializeNodePosition)
        {
            _graphRuntime = graphRuntime;
            _initializeNodePosition = initializeNodePosition;
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
                _initializeNodePosition(nodeId, menuPosition);
            }
        }
    }
}

#endif