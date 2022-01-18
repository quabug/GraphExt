using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class SelectionEntry<TNode> : IMenuEntry where TNode : INode<GraphRuntime<TNode>>
    {
        [NotNull] private readonly GraphRuntime<TNode> _graph;
        [NotNull] private readonly IReadOnlyDictionary<Node, NodeId> _nodes;
        [NotNull] private readonly IReadOnlyDictionary<Edge, EdgeId> _edges;

        public SelectionEntry(
            [NotNull] GraphRuntime<TNode> graph,
            [NotNull] IReadOnlyDictionary<Node, NodeId> nodes,
            [NotNull] IReadOnlyDictionary<Edge, EdgeId> edges
        )
        {
            _graph = graph;
            _nodes = nodes;
            _edges = edges;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            var edges = graph.selection?.OfType<Edge>();
            var nodes = graph.selection?.OfType<Node>();
            if (edges != null && edges.Any() || nodes != null && nodes.Any())
            {
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    foreach (var edge in edges)
                    {
                        var edgeId = _edges[edge];
                        _graph.Disconnect(input: edgeId.Input, output: edgeId.Output);
                    }

                    foreach (var node in nodes)
                    {
                        _graph.DeleteNode(_nodes[node]);
                    }
                });
                menu.AddSeparator("");
            }
        }
    }
}