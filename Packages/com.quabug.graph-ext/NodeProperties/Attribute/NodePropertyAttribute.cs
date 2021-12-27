using System;

namespace GraphExt
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NodePropertyAttribute : Attribute
    {
        public bool ReadOnly = false;
        public bool HideLabel = false;
        public bool HideValue = false;
        public string InputPort = null;
        public string OutputPort = null;
        public string Name = null;
        public bool SerializedField = true;
    }
}