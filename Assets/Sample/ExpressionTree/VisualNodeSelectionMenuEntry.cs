#if UNITY_EDITOR

using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

public class VisualNodeSelectionMenuEntry : SelectionEntry<IVisualNode>
{
    public VisualNodeSelectionMenuEntry([NotNull] GraphRuntime<IVisualNode> graph, [NotNull] IReadOnlyDictionary<Node, NodeId> nodes, [NotNull] IReadOnlyDictionary<Edge, EdgeId> edges) : base(graph, nodes, edges)
    {
    }
}

#endif