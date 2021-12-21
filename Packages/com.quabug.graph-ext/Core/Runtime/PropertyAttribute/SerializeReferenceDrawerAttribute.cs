using System;
using UnityEngine;

namespace GraphExt
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class SerializeReferenceDrawerAttribute : PropertyAttribute
    {
        public Type TypeRestrict;
        public string TypeRestrictBySiblingTypeName;
        public string TypeRestrictBySiblingProperty;
        public string RenamePatter;
        public bool DisplayAssemblyName = false;
        public bool AlphabeticalOrder = true;
        public string CategoryName;
        public bool Nullable = true;
        public string NullableVariable;
    }

}