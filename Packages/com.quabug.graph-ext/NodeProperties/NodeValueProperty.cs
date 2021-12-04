using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphExt
{
    public interface INodeReadOnlyValueProperty<out TValue> : INodeProperty
    {
        TValue Value { get; }
    }

    public class NodeReadOnlyValuePropertyView<TValue, TField> : BaseNodePropertyView, ITickableElement
        where TField : TextValueField<TValue>, new()
    {
        private readonly INodeReadOnlyValueProperty<TValue> _property;
        private readonly TField _field;

        public NodeReadOnlyValuePropertyView(INodeReadOnlyValueProperty<TValue> property)
        {
            _property = property;
            _field = new TField { value = property.Value, isReadOnly = true };
            SetField(_field);
        }

        public void Tick()
        {
            if (!EqualityComparer<TValue>.Default.Equals(_property.Value, _field.value))
                _field.SetValueWithoutNotify(_property.Value);
        }
    }

    public interface INodeValueProperty<TValue> : INodeProperty
    {
        TValue Value { get; set; }
    }

    public class NodeValuePropertyView<TValue, TField> : BaseNodePropertyView, IDisposable, ITickableElement
        where TField : TextValueField<TValue>, new()
    {
        private readonly INodeValueProperty<TValue> _property;
        private readonly TField _field;

        public NodeValuePropertyView(INodeValueProperty<TValue> property)
        {
            _property = property;
            _field = new TField { value = property.Value };
            _field.RegisterValueChangedCallback(OnValueChanged);
            SetField(_field);
        }

        public void Dispose()
        {
            _field.UnregisterValueChangedCallback(OnValueChanged);
        }

        private void OnValueChanged(ChangeEvent<TValue> e)
        {
            _property.Value = e.newValue;
        }

        public void Tick()
        {
            if (!EqualityComparer<TValue>.Default.Equals(_property.Value, _field.value))
                _field.SetValueWithoutNotify(_property.Value);
        }
    }

    public class NodeValuePropertyFactory<TValue, TField> : INodePropertyViewFactory
        where TField : TextValueField<TValue>, new()
    {
        public VisualElement Create(INodeProperty property)
        {
            if (property is INodeReadOnlyValueProperty<TValue> @readonly)
                return new NodeReadOnlyValuePropertyView<TValue, TField>(@readonly);
            if (property is INodeValueProperty<TValue> readwrite)
                return new NodeValuePropertyView<TValue, TField>(readwrite);
            return null;
        }
    }

    public class NodeIntProperty : NodeValuePropertyFactory<int, IntegerField> {}
    public class NodeLongProperty : NodeValuePropertyFactory<long, LongField> {}
    public class NodeFloatProperty : NodeValuePropertyFactory<float, FloatField> {}
    public class NodeDoubleProperty : NodeValuePropertyFactory<double, DoubleField> {}
}