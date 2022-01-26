namespace GraphExt.Editor
{
    public interface IWindowSystem {}

    public interface ITickableWindowSystem : IWindowSystem
    {
        void Tick();
    }

    public interface IInitializableWindowSystem : IWindowSystem
    {
        void Initialize();
    }
}