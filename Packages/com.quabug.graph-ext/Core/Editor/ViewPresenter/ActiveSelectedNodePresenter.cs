using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    /// <summary>
    /// Make `Selection` active object of corresponding selected node on `GraphView`
    /// </summary>
    /// <typeparam name="TNodeComponent">type of node object</typeparam>
    public class ActiveSelectedNodePresenter<TNodeComponent> : IViewPresenter
    {
        [NotNull] private readonly IReadOnlyDictionary<NodeId, Node> _nodes;
        [NotNull] private readonly IReadOnlyDictionary<NodeId, TNodeComponent> _nodeObjects;
        [NotNull] private readonly Action<TNodeComponent> _setActiveObject;
        private readonly HashSet<NodeId> _selectedNodes = new HashSet<NodeId>();

        public ActiveSelectedNodePresenter(
            [NotNull] IReadOnlyDictionary<NodeId, Node> nodes,
            [NotNull] IReadOnlyDictionary<NodeId, TNodeComponent> nodeObjects,
            [NotNull] Action<TNodeComponent> setActiveObject
        )
        {
            _nodes = nodes;
            _nodeObjects = nodeObjects;
            _setActiveObject = setActiveObject;
        }

        public void Tick()
        {
            var isChanged = false;
            foreach (var pair in _nodes)
            {
                var nodeId = pair.Key;
                var nodeView = pair.Value;
                if (nodeView.selected && !_selectedNodes.Contains(nodeId))
                {
                    _selectedNodes.Add(nodeId);
                    isChanged = true;
                }
                else if (!nodeView.selected && _selectedNodes.Contains(nodeId))
                {
                    _selectedNodes.Remove(nodeId);
                    isChanged = true;
                }
            }
            if (isChanged) SelectLast();
        }

        private void SelectLast()
        {
            var removed = new List<NodeId>();
            foreach (var nodeId in _selectedNodes)
            {
                if (_nodeObjects.TryGetValue(nodeId, out var node)) _setActiveObject(node);
                else removed.Add(nodeId);
            }

            foreach (var nodeId in removed) _selectedNodes.Remove(nodeId);
        }
    }
}