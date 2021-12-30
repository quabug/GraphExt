using System.Reflection;
using JetBrains.Annotations;

namespace GraphExt.Editor
{
    public class FieldInfoProperty<T> : IValueProperty<T>
    {
        public int Order => 0;
        private readonly object _target;
        private readonly FieldInfo _fieldInfo;

        public T Value
        {
            get => (T) _fieldInfo.GetValue(_target);
            set => _fieldInfo.SetValue(_target, value);
        }

        public FieldInfoProperty([NotNull] object target, [NotNull] FieldInfo fieldInfo)
        {
            _target = target;
            _fieldInfo = fieldInfo;
        }
    }

    public class ReadOnlyFieldInfoProperty<T> : IReadOnlyValueProperty<T>
    {
        public int Order => 0;
        private readonly object _target;
        private readonly FieldInfo _fieldInfo;

        public T Value => (T) _fieldInfo.GetValue(_target);

        public ReadOnlyFieldInfoProperty([NotNull] object target, [NotNull] FieldInfo fieldInfo)
        {
            _target = target;
            _fieldInfo = fieldInfo;
        }
    }
}