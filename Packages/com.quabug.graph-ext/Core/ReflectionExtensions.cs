using System;
using System.Reflection;

namespace GraphExt
{
    public static class ReflectionExtensions
    {
        public static object GetValue(this MemberInfo memberInfo, object target)
        {
            return memberInfo switch
            {
                FieldInfo fi => fi.GetValue(target),
                PropertyInfo pi => pi.GetValue(target),
                _ => throw new NotImplementedException()
            };
        }

        public static T GetValue<T>(this MemberInfo memberInfo, object target)
        {
            return (T)GetValue(memberInfo, target);
        }
    }
}