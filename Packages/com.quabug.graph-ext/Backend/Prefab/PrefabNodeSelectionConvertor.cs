#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class PrefabNodeSelectionConvertor<TNodeId, TNodeView, TNodeComponent> : SyncSelectionGraphElementPresenter.IConvertor
        where TNodeComponent : Component
        where TNodeView : class, ISelectable
    {
        private readonly IReadOnlyBiDictionary<TNodeId, TNodeView> _views;
        private readonly IReadOnlyBiDictionary<TNodeId, TNodeComponent> _nodes;

        public PrefabNodeSelectionConvertor(IReadOnlyBiDictionary<TNodeId, TNodeView> views, IReadOnlyBiDictionary<TNodeId, TNodeComponent> nodes)
        {
            _views = views;
            _nodes = nodes;
        }

        public Object ConvertGraphSelectableToObject(ISelectable selectable)
        {
            if (!(selectable is TNodeView node)) return null;
            if (!_views.Reverse.TryGetValue(node, out var nodeId)) return null;
            if (!_nodes.TryGetValue(nodeId, out var nodeComponent)) return null;
            return nodeComponent == null ? null : nodeComponent.gameObject;
        }

        public ISelectable ConvertObjectToGraphSelectable(Object @object)
        {
            var nodeComponent = @object is GameObject node ? node.GetComponent<TNodeComponent>() : null;
            if (nodeComponent == null || !_nodes.Reverse.TryGetValue(nodeComponent, out var nodeId)) return null;
            if (!_views.TryGetValue(nodeId, out var nodeView)) return null;
            return nodeView;
        }
    }
}

#endif