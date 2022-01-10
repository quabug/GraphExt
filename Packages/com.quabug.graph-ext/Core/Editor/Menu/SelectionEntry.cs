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
        [NotNull] private readonly IGraphElements<NodeId, Node> _nodes;
        [NotNull] private readonly IGraphElements<EdgeId, Edge> _edges;

        public SelectionEntry(
            [NotNull] GraphRuntime<TNode> graph,
            [NotNull] IGraphElements<NodeId, Node> nodes,
            [NotNull] IGraphElements<EdgeId, Edge> edges
        )
        {
            _graph = graph;
            _nodes = nodes;
            _edges = edges;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            if (graph.selection != null && graph.selection.Any())
            {
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    if (graph.selection != null)
                    {
                        foreach (var edge in graph.selection.OfType<Edge>())
                        {
                            var edgeId = _edges[edge];
                            _graph.Disconnect(input: edgeId.Input, output: edgeId.Output);
                        }

                        foreach (var node in graph.selection.OfType<Node>())
                        {
                            _graph.DeleteNode(_nodes[node]);
                        }
                    }
                });
                menu.AddSeparator("");
            }
        }
    }
}