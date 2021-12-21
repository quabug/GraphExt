using System.Linq;
using GraphExt;
using GraphExt.Memory;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Node = UnityEditor.Experimental.GraphView.Node;

public class PrintValueMenu : IMenuEntry
{
    public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
    {
        if (graph.selection?.FirstOrDefault() is Node node && graph.Module is Graph module)
        {
            var nodeId = graph.GetNodeId(node);
            menu.AddItem(new GUIContent("Print Node Value"), true, () =>
            {
                var visualNode = module[nodeId].Inner as IVisualNode;
                var value = visualNode?.GetValue(module);
                Debug.Log($"value = {value}");
            });
        }
    }
}