using System;
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
            _Container?.Dispose();
            _Container = null;
            _Systems = null;
            _MenuBuilder = null;
        }

        protected void Install(Container container, TypeContainers typeContainers)
        {
            container.RegisterInstance(this).AsSelf();
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
    }
}