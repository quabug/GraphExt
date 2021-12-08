using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Memory
{
    public class MemoryNodeMenuEntry : IMenuEntry
    {
        public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            if (!(graph.Module is Graph module)) return;

            var memoryNodes = TypeCache.GetTypesDerivedFrom<IMemoryNode>();
            foreach (var nodeType in memoryNodes
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .OrderBy(type => type.Name))
            {
                menu.AddItem(new GUIContent($"Node/{nodeType.Name}"), false, () => CreateNode(nodeType));
            }

            void CreateNode(Type nodeType)
            {
                module.NodeList.Add(new Node((IMemoryNode) Activator.CreateInstance(nodeType)));
            }
        }
    }
}