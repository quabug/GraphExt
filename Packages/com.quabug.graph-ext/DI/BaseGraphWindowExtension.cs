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

        [SerializedType(typeof(IWindowSystem), Nullable = false, InstantializableType = true, RenamePatter = @"\w*\.||")]
        public string[] AdditionalWindowSystems;

        protected Container _Container;
        protected MenuBuilder _MenuBuilder;
        protected WindowSystems _Systems;

        public void RecreateGraphView(VisualElement root)
        {
            _Container = new Container();
            Install(_Container);
            InstantiateSystems(_Container);
            RecreateGraphView(root, _Container);
        }

        public void Tick()
        {
            _Systems.Tick();
        }

        public void Clear()
        {
            _MenuBuilder?.Dispose();
            _Systems?.Dispose();
            _Container?.Dispose();
        }

        protected virtual void Install(Container container)
        {
            container.RegisterInstance(this);
            container.RegisterTypeNameArraySingleton<IWindowSystem>(AdditionalWindowSystems);
            foreach (var installer in Installers) installer.Install(container);
            foreach (var installer in MenuEntries) installer.Install(container);
        }

        protected virtual void RecreateGraphView(VisualElement root, Container container)
        {
            var graph = root.Q<UnityEditor.Experimental.GraphView.GraphView>();
            graph?.parent.Remove(graph);
            root.Add(container.Resolve<UnityEditor.Experimental.GraphView.GraphView>());
        }

        protected virtual void InstantiateSystems(Container container)
        {
            _Systems = container.Instantiate<WindowSystems>();
            _MenuBuilder = container.Instantiate<MenuBuilder>();
        }

        protected class WindowSystems : IDisposable
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