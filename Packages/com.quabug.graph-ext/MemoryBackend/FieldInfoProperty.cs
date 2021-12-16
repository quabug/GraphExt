using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace GraphExt.Memory
{
    public class FieldInfoProperty<T> : IValueProperty<T>
    {
        private readonly object _target;
        private readonly FieldInfo _fieldInfo;
        public IEnumerable<IPortModule> Ports => Enumerable.Empty<IPortModule>();

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
        private readonly object _target;
        private readonly FieldInfo _fieldInfo;
        public IEnumerable<IPortModule> Ports => Enumerable.Empty<IPortModule>();

        public T Value => (T) _fieldInfo.GetValue(_target);

        public ReadOnlyFieldInfoProperty([NotNull] object target, [NotNull] FieldInfo fieldInfo)
        {
            _target = target;
            _fieldInfo = fieldInfo;
        }
    }
}