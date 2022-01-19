using System;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    [AttributeUsage(AttributeTargets.Field)]
    [BaseTypeRequired(typeof(string))]
    internal class SerializedTypeAttribute : PropertyAttribute
    {
        public Type BaseType;
        public string RenamePatter;
        public bool DisplayAssemblyName = false;
        public bool AlphabeticalOrder = true;
        public string CategoryName;
        public bool Nullable = true;
        public string Where = "";
        public bool InstantializableType = false;
        public SerializedTypeAttribute(Type baseType = null) => BaseType = baseType;
    }

}