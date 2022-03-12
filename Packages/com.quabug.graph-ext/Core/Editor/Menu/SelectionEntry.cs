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
            var edges = graph.selection?.OfType<Edge>() ?? Enumerable.Empty<Edge>();
            var edgeIds = edges.Select(edge => _edges[edge]).ToArray();
            var nodes = graph.selection?.OfType<Node>() ?? Enumerable.Empty<Node>();
            var nodeIds = nodes.Select(node => _nodes[node]).ToArray();
            if (edgeIds.Any() || nodeIds.Any())
            {
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    foreach (var edge in edgeIds)
                    {
                        _graph.Disconnect(input: edge.Input, output: edge.Output);
                    }

                    foreach (var node in nodeIds)
                    {
                        _graph.DeleteNode(node);
                    }
                });
                menu.AddSeparator("");
            }
        }
    }
}