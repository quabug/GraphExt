namespace GraphExt.Editor
{
    public interface IContainerBuilder
    {
        IContainerBuilder Register<T>();
        IContainerBuilder Register(object instance);
    }
}