using System.Collections.Generic;
using System.Linq;
using GraphExt.Editor;
using UnityEngine;

namespace GraphExt
{
    public class ScriptableObjectNodePositions<TNode, TNodeScriptableObject> : IViewModuleElements<NodeId, Vector2>
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        private readonly GraphScriptableObject<TNode, TNodeScriptableObject> _graph;
        public ScriptableObjectNodePositions(GraphScriptableObject<TNode, TNodeScriptableObject> graph)
        {
            _graph = graph;
        }

        public Vector2 this[in NodeId id]
        {
            get => _graph[id].Position;
            set => _graph[id].Position = value;
        }

        public bool Has(in NodeId id)
        {
            return _graph.NodeObjectMap.ContainsKey(id);
        }

        public IEnumerable<(NodeId id, Vector2 data)> Elements => _graph.Nodes.Select(node => (node.Id, node.Position));

        public void Add(in NodeId id, Vector2 data)
        {
            if (Has(id)) this[id] = data;
        }

        public void Remove(in NodeId id)
        {
            if (Has(id)) this[id] = Vector2.zero;
        }
    }
}