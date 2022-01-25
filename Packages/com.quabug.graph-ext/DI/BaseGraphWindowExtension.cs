using System;
using System.Linq;
using OneShot;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    [Serializable]
    public class BaseGraphWindowExtension : IGraphWindowExtension
    {
        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IGraphInstaller[] Installers;

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IMenuEntryInstaller[] MenuEntries;

        private Container _container;
        private MenuBuilder _menuBuilder;
        private WindowSystems _systems;

        public void RecreateGraphView(VisualElement root)
        {
            _container = new Container();
            Install(_container);

            // var graphView = _container.Resolve<UnityEditor.Experimental.GraphView.GraphView>();
            // ReplaceGraphView(graphView);
            InstantiateSystems(_container);
        }

        public void Tick()
        {
            _systems.Tick();
        }

        public void Clear()
        {
            _menuBuilder?.Dispose();
            _systems?.Dispose();
            _container?.Dispose();
        }

        protected virtual void Install(Container container)
        {
            container.RegisterInstance(this);
            foreach (var installer in Installers) installer.Install(container);
            foreach (var installer in MenuEntries.Reverse()) installer.Install(container);
        }

        protected virtual void RecreateGraphView(VisualElement root, Container container)
        {
            var graph = root.Q<UnityEditor.Experimental.GraphView.GraphView>();
            graph?.parent.Remove(graph);
            root.Add(container.Resolve<UnityEditor.Experimental.GraphView.GraphView>());
        }

        protected virtual void InstantiateSystems(Container container)
        {
            _systems = container.Resolve<WindowSystems>();
            _menuBuilder = container.Instantiate<MenuBuilder>();
        }

        class WindowSystems : IDisposable
        {
            private readonly IWindowSystem[] _systems;

            public WindowSystems(IWindowSystem[] systems)
            {
                _systems = systems;
            }

            public void Tick()
            {
                foreach (var system in _systems.OfType<ITickableWindowSystem>()) system.Tick();
            }

            public void Dispose()
            {
                foreach (var system in _systems.OfType<IDisposable>()) system.Dispose();
            }
        }
    }
}