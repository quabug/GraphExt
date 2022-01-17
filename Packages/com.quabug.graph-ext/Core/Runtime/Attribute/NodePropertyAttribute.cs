using System;

namespace GraphExt
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NodePropertyAttribute : Attribute
    {
        /// <summary>
        /// read only
        /// will force set to true if member is `readonly` field, const field or getter-only property.
        /// </summary>
        public bool ReadOnly = false;

        /// <summary>
        /// hide label property
        /// </summary>
        public bool HideLabel = false;

        /// <summary>
        /// hide value property
        /// </summary>
        public bool HideValue = false;

        /// <summary>
        /// reference name of input port
        /// will hide referenced port from its own property
        /// </summary>
        public string InputPort = null;

        /// <summary>
        /// reference name of output port
        /// will hide referenced port from its own property
        /// </summary>
        public string OutputPort = null;

        /// <summary>
        /// override name for UI only
        /// </summary>
        public string Name = null;

        /// <summary>
        /// serialized by Unity3D
        /// will force set to false if not able to serialized by Unity3D
        /// </summary>
        public bool SerializedField = true;

        /// <summary>
        /// override default node property creator by this custom one.
        /// </summary>
        public Type CustomFactory = null;

        public const System.Reflection.BindingFlags BindingFlags =
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.FlattenHierarchy;
    }
}