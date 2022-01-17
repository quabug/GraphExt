namespace GraphExt
{
    public interface INodeProperty
    {
        int Order { get; }
    }

#if UNITY_EDITOR
    public interface INodePropertyFactory
    {
        INodeProperty Create(
            System.Reflection.MemberInfo memberInfo,
            object nodeObj,
            NodeId nodeId,
            UnityEditor.SerializedProperty fieldProperty = null
        );
    }
#endif
}