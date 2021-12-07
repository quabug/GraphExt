using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class SerializedProperty : INodeProperty
    {
        public UnityEditor.SerializedProperty Value;

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property)
            {
                return property is SerializedProperty p ? new View(p) : null;
            }
        }

        public class View : BaseNodePropertyView
        {
            public View(SerializedProperty property)
            {
                var field = new PropertyField(property.Value);
                SetField(field);
            }
        }
    }
}