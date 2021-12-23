#if UNITY_EDITOR

using System;
using System.Linq;
using GraphExt.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Memory
{
    public class MemoryNodeMenuEntry : IMenuEntry
    {
        public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            if (!(graph.Module is MemoryGraphBackend module)) return;

            var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            var memoryNodes = TypeCache.GetTypesDerivedFrom<IMemoryNode>();
            foreach (var nodeType in memoryNodes
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .OrderBy(type => type.Name))
            {
                menu.AddItem(new GUIContent($"Node/{nodeType.Name}"), false, () => CreateNode(nodeType));
            }

            void CreateNode(Type nodeType)
            {
                var innerNode = (IMemoryNode)Activator.CreateInstance(nodeType);
                var node = new MemoryGraphBackend.Node(innerNode, Guid.NewGuid(), menuPosition);
                module.AddNode(node);
            }
        }
    }
}

#endif