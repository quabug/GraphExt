using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class SerializedFieldProperty : INodeProperty
    {
        public int Order => 0;
        public SerializedProperty Value;

        public SerializedFieldProperty(SerializedProperty value)
        {
            Value = value;
        }

        private class Factory : SingleNodePropertyViewFactory<SerializedFieldProperty>
        {
            protected override VisualElement CreateView(Node node, SerializedFieldProperty fieldProperty, INodePropertyViewFactory _)
            {
                return new PropertyFieldWithoutLabel(fieldProperty.Value);
            }
        }
    }

    public class PropertyFieldWithoutLabel : PropertyField
    {
        public PropertyFieldWithoutLabel(SerializedProperty property) : base(property, label: null)
        {
            this.BindProperty(property.serializedObject);
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            // HACK: hide label of PropertyField
            if (evt.GetType().Name == "SerializedPropertyBindEvent" /* cannot access `nameof` internal class */)
                this.Q<Label>().style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }
    }
}