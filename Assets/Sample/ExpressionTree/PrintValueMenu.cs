#if UNITY_EDITOR

using System.Linq;
using GraphExt.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Node = UnityEditor.Experimental.GraphView.Node;

public class PrintValueMenu : IMenuEntry
{
    public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
    {
        if (graph.selection?.FirstOrDefault() is Node node && graph.Module is GraphViewModule<IVisualNode> module)
        {
            var nodeId = graph.GetNodeId(node);
            menu.AddItem(new GUIContent("Print Node Value"), true, () =>
            {
                var visualNode = module.Runtime[nodeId];
                var value = visualNode.GetValue(module.Runtime);
                Debug.Log($"value = {value}");
            });
        }
    }
}

#endif