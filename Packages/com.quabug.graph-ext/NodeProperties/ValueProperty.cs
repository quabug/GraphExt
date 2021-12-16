using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphExt
{
    public interface IReadOnlyValueProperty<out TValue> : INodeProperty
    {
        TValue Value { get; }
    }

    public class ReadOnlyValueProperty<TValue> : IReadOnlyValueProperty<TValue>
    {
        private readonly Func<TValue> _getter;
        public TValue Value => _getter();
        public ReadOnlyValueProperty(Func<TValue> getter) => _getter = getter;
    }

    public class ReadOnlyValuePropertyView<TValue, TField> : VisualElement, ITickableElement
        where TField : TextValueField<TValue>, new()
    {
        private readonly IReadOnlyValueProperty<TValue> _property;
        private readonly TField _field;

        public ReadOnlyValuePropertyView(IReadOnlyValueProperty<TValue> property)
        {
            _property = property;
            _field = new TField { value = property.Value, isReadOnly = true };
            Add(_field);
        }

        public void Tick()
        {
            if (!EqualityComparer<TValue>.Default.Equals(_property.Value, _field.value))
                _field.SetValueWithoutNotify(_property.Value);
        }
    }

    public interface IValueProperty<TValue> : INodeProperty
    {
        TValue Value { get; set; }
    }

    public class ValueProperty<TValue> : IValueProperty<TValue>
    {
        private readonly Func<TValue> _getter;
        private readonly Action<TValue> _setter;

        public TValue Value
        {
            get => _getter();
            set => _setter(value);
        }

        public ValueProperty(Func<TValue> getter, Action<TValue> setter)
        {
            _getter = getter;
            _setter = setter;
        }
    }

    public class ValuePropertyView<TValue, TField> : VisualElement, IDisposable, ITickableElement
        where TField : TextValueField<TValue>, new()
    {
        private readonly IValueProperty<TValue> _property;
        private readonly TField _field;

        public ValuePropertyView(IValueProperty<TValue> property)
        {
            _property = property;
            _field = new TField { value = property.Value };
            _field.RegisterValueChangedCallback(OnValueChanged);
            Add(_field);
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

    public class ValuePropertyFactory<TValue, TField> : INodePropertyViewFactory
        where TField : TextValueField<TValue>, new()
    {
        public VisualElement Create(INodeProperty property, INodePropertyViewFactory _)
        {
            if (property is IReadOnlyValueProperty<TValue> @readonly)
                return new ReadOnlyValuePropertyView<TValue, TField>(@readonly);
            if (property is IValueProperty<TValue> readwrite)
                return new ValuePropertyView<TValue, TField>(readwrite);
            return null;
        }
    }

    public class IntPropertyFactory : ValuePropertyFactory<int, IntegerField> {}
    public class LongPropertyFactory : ValuePropertyFactory<long, LongField> {}
    public class FloatPropertyFactory : ValuePropertyFactory<float, FloatField> {}
    public class DoublePropertyFactory : ValuePropertyFactory<double, DoubleField> {}
}