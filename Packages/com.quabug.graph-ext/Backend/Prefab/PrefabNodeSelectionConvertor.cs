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
            return selectable is TNodeView node ? _nodes[_views.Reverse[node]].gameObject : null;
        }

        public ISelectable ConvertObjectToGraphSelectable(Object @object)
        {
            var nodeComponent = @object is GameObject node ? node.GetComponent<TNodeComponent>() : null;
            return nodeComponent == null ? null : _views[_nodes.Reverse[nodeComponent]];
        }
    }
}

#endif