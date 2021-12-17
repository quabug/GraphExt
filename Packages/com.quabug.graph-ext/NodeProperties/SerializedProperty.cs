using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class SerializedProperty : INodeProperty
    {
        public UnityEditor.SerializedProperty Value;

        public class Factory : NodePropertyViewFactory<SerializedProperty>
        {
            protected override VisualElement Create(SerializedProperty property, INodePropertyViewFactory _)
            {
                return new PropertyField(property.Value);
            }
        }
    }
}