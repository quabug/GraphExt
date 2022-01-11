#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PrintValueMenu : IMenuEntry
{
    [NotNull] private readonly GraphRuntime<IVisualNode> _graphRuntime;
    [NotNull] private readonly IReadOnlyDictionary<Node, NodeId> _nodeViews;

    public PrintValueMenu(
        [NotNull] GraphRuntime<IVisualNode> graphRuntime,
        [NotNull] IReadOnlyDictionary<Node, NodeId> nodeViews
    )
    {
        _graphRuntime = graphRuntime;
        _nodeViews = nodeViews;
    }

    public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
    {
        if (graph.selection?.FirstOrDefault() is Node node)
        {
            var nodeId = _nodeViews[node];
            menu.AddItem(new GUIContent("Print Node Value"), true, () =>
            {
                var visualNode = _graphRuntime[nodeId];
                var value = visualNode.GetValue(_graphRuntime);
                Debug.Log($"value = {value}");
            });
        }
    }
}

#endif