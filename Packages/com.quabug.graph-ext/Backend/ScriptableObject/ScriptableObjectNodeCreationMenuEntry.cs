#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class ScriptableObjectNodeCreationMenuEntry<TNode, TNodeScriptableObject> : IMenuEntry
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            // if (!(graph.Module is ScriptableObjectGraphViewModule<TNode, TNodeScriptableObject> _)) return;
            //
            // var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            // var nodes = TypeCache.GetTypesDerivedFrom<TNode>();
            // foreach (var nodeType in nodes
            //     .Where(type => !type.IsAbstract && !type.IsGenericType)
            //     .OrderBy(type => type.Name))
            // {
            //     menu.AddItem(new GUIContent($"{typeof(TNode).Name}/{nodeType.Name}"), false, () => CreateNode(nodeType));
            // }
            //
            // void CreateNode(Type nodeType)
            // {
            //     if (graph.Module is ScriptableObjectGraphViewModule<TNode, TNodeScriptableObject> viewModule)
            //     {
            //         viewModule.AddScriptableObjectNode(Guid.NewGuid(), (TNode)Activator.CreateInstance(nodeType), menuPosition);
            //     }
            // }
        }
    }
}

#endif
