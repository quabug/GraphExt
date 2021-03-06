using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace GraphExt.Editor
{
    public interface IReadOnlyValueProperty<out TValue> : INodeProperty
    {
        TValue Value { get; }
    }

    public class ReadOnlyValueProperty<TValue> : IReadOnlyValueProperty<TValue>
    {
        public int Order { get; set; } = 0;
        private readonly Func<TValue> _getter;
        public TValue Value => _getter();
        public ReadOnlyValueProperty(Func<TValue> getter) => _getter = getter;
    }

    public interface IValueProperty<TValue> : INodeProperty
    {
        TValue Value { get; set; }
    }

    public class ValueProperty<TValue> : IValueProperty<TValue>
    {
        public int Order { get; set; } = 0;
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

    public class ValuePropertyFactory<TValue, TField> : SingleNodePropertyViewFactory<IValueProperty<TValue>>
        where TField : TextValueField<TValue>, new()
    {
        protected override VisualElement CreateView(Node node, IValueProperty<TValue> property, INodePropertyViewFactory _)
        {
            return new ValuePropertyView<TValue, TField>(property);
        }
    }

    public class IntPropertyFactory : ValuePropertyFactory<int, IntegerField> {}
    public class LongPropertyFactory : ValuePropertyFactory<long, LongField> {}
    public class FloatPropertyFactory : ValuePropertyFactory<float, FloatField> {}
    public class DoublePropertyFactory : ValuePropertyFactory<double, DoubleField> {}

    public class ReadOnlyValuePropertyFactory<TValue, TField> : SingleNodePropertyViewFactory<IReadOnlyValueProperty<TValue>>
        where TField : TextValueField<TValue>, new()
    {
        protected override VisualElement CreateView(Node node, IReadOnlyValueProperty<TValue> property, INodePropertyViewFactory _)
        {
            return new ReadOnlyValuePropertyView<TValue, TField>(property);
        }
    }

    public class IntReadOnlyPropertyFactory : ValuePropertyFactory<int, IntegerField> {}
    public class LongReadOnlyPropertyFactory : ValuePropertyFactory<long, LongField> {}
    public class FloatReadOnlyPropertyFactory : ValuePropertyFactory<float, FloatField> {}
    public class DoubleReadOnlyPropertyFactory : ValuePropertyFactory<double, DoubleField> {}
}