using OneShot;

namespace GraphExt.Editor
{
    public interface IGraphInstaller
    {
        void Install(Container container, TypeContainers typeContainers);
    }
}