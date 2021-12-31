using System;

namespace GraphExt
{
    public enum PortDirection
    {
        Input, Output, Invalid
    }

    public enum PortOrientation
    {
        Horizontal, Vertical, Invalid
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class NodePortAttribute : Attribute
    {
        /// <summary>
        /// true: hide entire port
        /// will force set to true if this is a property port
        /// </summary>
        public bool Hide = false;

        /// <summary>
        /// true: hide label only
        /// </summary>
        public bool HideLabel = false;
        public Type PortType = null;

        /// <summary>
        /// input or output
        /// </summary>
        public PortDirection Direction = PortDirection.Invalid;

        /// <summary>
        /// restrict number of connections
        /// </summary>
        public int Capacity = 0;

        /// <summary>
        /// horizontal or vertical
        /// </summary>
        public PortOrientation Orientation = PortOrientation.Horizontal;

        /// <summary>
        /// override port name for UI only
        /// </summary>
        public string DisplayName = null;

        /// <summary>
        /// identity the port
        /// useful for renaming port
        /// </summary>
        // public string Id = Guid.NewGuid().ToString();

        public const System.Reflection.BindingFlags BindingFlags =
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.FlattenHierarchy;
    }
}