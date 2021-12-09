using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt
{
    public sealed class NodeView : Node, ITickableElement
    {
        private readonly INodeModule _nodeModule;
        private readonly GraphConfig _config;
        private readonly VisualElement _contentContainer;
        private readonly IDictionary<INodeProperty, VisualElement> _properties = new Dictionary<INodeProperty, VisualElement>();

        public NodeView(INodeModule nodeModule, GraphConfig config)
            : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml"))
        {
            _nodeModule = nodeModule;
            _config = config;
            style.left = nodeModule.Position.x;
            style.top = nodeModule.Position.y;
            _contentContainer = this.Q<VisualElement>("contents");
        }

        public void Tick()
        {
            var currentProperties = new HashSet<INodeProperty>(_properties.Keys);
            foreach (var property in _nodeModule.Properties)
            {
                if (currentProperties.Contains(property)) currentProperties.Remove(property);
                else AddProperty(property);
            }

            foreach (var removed in currentProperties)
            {
                _contentContainer.Remove(_properties[removed]);
            }

            void AddProperty(INodeProperty property)
            {
                var view = _config.CreatePropertyView(property);
                if (view != null)
                {
                    _contentContainer.Add(view);
                    _properties.Add(property, view);
                }
            }
        }
    }
}