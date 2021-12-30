using System;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeSelector : INodeProperty
    {
        public int Order => 0;
        public event Action<bool> OnSelectChanged;

        private class View : VisualElement, ITickableElement
        {
            private readonly UnityEditor.Experimental.GraphView.Node _node;
            private readonly NodeSelector _selector;
            private bool _isSelected;

            public View(UnityEditor.Experimental.GraphView.Node node, NodeSelector selector)
            {
                _node = node;
                _selector = selector;
                _isSelected = _node.selected;
            }

            public void Tick()
            {
                if (_isSelected != _node.selected)
                {
                    _isSelected = _node.selected;
                    _selector.OnSelectChanged?.Invoke(_isSelected);
                }
            }
        }

        private class Factory : NodePropertyViewFactory<NodeSelector>
        {
            protected override VisualElement Create(UnityEditor.Experimental.GraphView.Node node, NodeSelector property, INodePropertyViewFactory factory)
            {
                return new View(node, property);
            }
        }
    }
}