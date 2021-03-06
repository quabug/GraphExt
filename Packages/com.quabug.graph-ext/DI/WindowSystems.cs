using System;
using System.Linq;

namespace GraphExt.Editor
{
    public class WindowSystems
    {
        private readonly IWindowSystem[] _systems;

        public WindowSystems(IWindowSystem[] systems)
        {
            _systems = systems;
        }

        public void Initialize()
        {
            foreach (var system in _systems.OfType<IInitializableWindowSystem>()) system.Initialize();
        }

        public void Tick()
        {
            foreach (var system in _systems.OfType<ITickableWindowSystem>()) system.Tick();
        }
    }
}