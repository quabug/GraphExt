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
        protected VisualElement _Root;
        protected TypeContainers _typeContainers;

        public void Recreate(VisualElement root)
        {
            _Root = root;
            _Container = new Container();
            Recreate();
        }

        protected virtual void Recreate()
        {
            Install(_Container, new TypeContainers());
            InstantiateSystems(_Container);
            ReplaceGraphView(_Container.Resolve<UnityEditor.Experimental.GraphView.GraphView>());
        }

        public void Tick()
        {
            _Systems?.Tick();
        }

        public void Clear()
        {
            _MenuBuilder?.Dispose();
            _MenuBuilder = null;
            _Systems?.Dispose();
            _Systems = null;
            _Container?.Dispose();
            _Container = null;
        }

        protected void Install(Container container, TypeContainers typeContainers)
        {
            container.RegisterInstance(this);
            container.RegisterTypeNameArraySingleton<IWindowSystem>(AdditionalWindowSystems);
            foreach (var installer in Installers) installer.Install(container, typeContainers);
            foreach (var installer in MenuEntries) installer.Install(container);
        }

        protected void RemoveGraphView()
        {
            var graph = _Root.Q<UnityEditor.Experimental.GraphView.GraphView>();
            if (graph != null) _Root.Remove(graph);
        }

        protected void ReplaceGraphView(UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            RemoveGraphView();
            _Root.Add(graphView);
        }

        protected void InstantiateSystems(Container container)
        {
            _Systems = container.Instantiate<WindowSystems>();
            _MenuBuilder = container.Instantiate<MenuBuilder>();
            _Systems.Initialize();
        }

        protected class WindowSystems : IDisposable
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

            public void Dispose()
            {
                foreach (var system in _systems.OfType<IDisposable>()) system.Dispose();
            }
        }
    }
}