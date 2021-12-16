using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class SerializedProperty : INodeProperty
    {
        public UnityEditor.SerializedProperty Value;

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory _)
            {
                return property is SerializedProperty p ? new PropertyField(p.Value) : null;
            }
        }
    }
}