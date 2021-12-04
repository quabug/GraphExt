using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class NodeSerializedProperty : INodeProperty
    {
        public SerializedProperty Value;

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property)
            {
                return property is NodeSerializedProperty p ? new View(p) : null;
            }
        }

        public class View : BaseNodePropertyView
        {
            public View(NodeSerializedProperty property)
            {
                var field = new PropertyField(property.Value);
                SetField(field);
            }
        }
    }
}