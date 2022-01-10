using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace GraphExt.Editor
{
    public class PropertyInfoProperty<T> : IValueProperty<T>
    {
        public int Order => 0;
        private readonly object _target;
        private readonly MethodInfo _getter;
        private readonly MethodInfo _setter;

        public T Value
        {
            get => (T)_getter.Invoke(_target, Array.Empty<object>());
            set => _setter.Invoke(_target, new object[] { value });
        }

        public PropertyInfoProperty([NotNull] object target, [NotNull] PropertyInfo propertyInfo)
        {
            Assert.AreEqual(propertyInfo.PropertyType, typeof(T));
            _target = target;

            _getter = propertyInfo.GetGetMethod(nonPublic: true);
            Assert.IsNotNull(_getter);

            _setter = propertyInfo.GetSetMethod(nonPublic: true);
            Assert.IsNotNull(_setter);
        }
    }

    public class ReadOnlyPropertyInfoProperty<T> : IReadOnlyValueProperty<T>
    {
        public int Order => 0;
        private readonly object _target;
        private readonly MethodInfo _getter;

        public T Value => (T)_getter.Invoke(_target, Array.Empty<object>());

        public ReadOnlyPropertyInfoProperty([NotNull] object target, [NotNull] PropertyInfo propertyInfo)
        {
            Assert.AreEqual(propertyInfo.PropertyType, typeof(T));
            _target = target;

            _getter = propertyInfo.GetGetMethod(nonPublic: true);
            Assert.IsNotNull(_getter);
        }
    }
}