using UnityEngine.UIElements;

namespace GraphExt
{
    public class SerializedProperty : INodeProperty
    {
        public UnityEditor.SerializedProperty Value;

#if UNITY_EDITOR
        public class Factory : NodePropertyViewFactory<SerializedProperty>
        {
            protected override VisualElement Create(SerializedProperty property, INodePropertyViewFactory _)
            {
                return new UnityEditor.UIElements.PropertyField(property.Value);
            }
        }
#endif
    }
}