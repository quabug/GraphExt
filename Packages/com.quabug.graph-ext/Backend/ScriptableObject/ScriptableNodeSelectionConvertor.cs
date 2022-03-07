#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class ScriptableNodeSelectionConvertor<TNodeId, TNodeView, TNodeComponent> : SyncSelectionGraphElementPresenter.IConvertor
        where TNodeComponent : ScriptableObject
        where TNodeView : class, ISelectable
    {
        private readonly IReadOnlyBiDictionary<TNodeId, TNodeView> _views;
        private readonly IReadOnlyBiDictionary<TNodeId, TNodeComponent> _nodes;

        public ScriptableNodeSelectionConvertor(IReadOnlyBiDictionary<TNodeId, TNodeView> views, IReadOnlyBiDictionary<TNodeId, TNodeComponent> nodes)
        {
            _views = views;
            _nodes = nodes;
        }

        public Object ConvertGraphSelectableToObject(ISelectable selectable)
        {
            return selectable is TNodeView node ? _nodes[_views.Reverse[node]] : null;
        }

        public ISelectable ConvertObjectToGraphSelectable(Object @object)
        {
            var node = @object as TNodeComponent;
            return node == null ? null : _views[_nodes.Reverse[node]];
        }
    }
}

#endif
