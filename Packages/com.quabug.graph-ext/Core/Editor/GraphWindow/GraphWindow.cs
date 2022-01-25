using System;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public sealed class GraphWindow : EditorWindow
    {
        [SerializeField] private GraphConfig _config;
        public GraphConfig Config
        {
            set
            {
                if (_config != value)
                {
                    _config = value;
                    CreateGUI();
                }
            }
        }

        private readonly Lazy<VisualElement> _graphRoot;
        // private Container _container;
        // private MenuBuilder _menuBuilder;
        // private IWindowSystem[] _systems = Array.Empty<IWindowSystem>();

        public GraphWindow()
        {
            _graphRoot = new Lazy<VisualElement>(LoadVisualTree);
        }

        public void CreateGUI()
        {
            if (_config)
            {
                var graphViewRoot = _graphRoot.Value.Q<VisualElement>("graph-content");
                _config.GraphWindowExtension.RecreateGraphView(graphViewRoot);
            }
        }

        private void Update()
        {
            if (_config) _config.GraphWindowExtension.Tick();
            // foreach (var presenter in _systems.OfType<ITickableWindowSystem>()) presenter.Tick();
        }

        private void OnDestroy()
        {
            if (_config) _config.GraphWindowExtension.Clear();
        }
        //
        // public void RecreateGUI()
        // {
        //     RecreateGUI(new Container());
        // }
        //
        // public void RecreateGUI(Container container)
        // {
        //     if (_container != null)
        //     {
        //         _menuBuilder.Dispose();
        //         foreach (var disposable in _systems.OfType<IDisposable>()) disposable.Dispose();
        //         _container.Dispose();
        //     }
        //
        //     _container = container;
        //     _container.RegisterInstance(this);
        //     foreach (var installer in _config.Installers) installer.Install(_container);
        //     foreach (var installer in _config.MenuEntries.Reverse()) installer.Install(_container);
        //
        //     // var graphView = _container.Resolve<UnityEditor.Experimental.GraphView.GraphView>();
        //     // ReplaceGraphView(graphView);
        //     _systems = _container.ResolveGroup<IWindowSystem>().ToArray();
        //     _menuBuilder = _container.Instantiate<MenuBuilder>();
        // }
        //
        // private void RemoveGraphView()
        // {
        //     var graph = _graphRoot.Value.Q<UnityEditor.Experimental.GraphView.GraphView>();
        //     graph?.parent.Remove(graph);
        // }
        //
        // private void ReplaceGraphView(UnityEditor.Experimental.GraphView.GraphView graphView)
        // {
        //     RemoveGraphView();
        //     AddGraphView(graphView);
        // }

        private VisualElement LoadVisualTree()
        {
            var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "GraphWindow.uxml");
            var ussPath = Path.Combine(relativeDirectory, "GraphWindow.uss");

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(rootVisualElement);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            rootVisualElement.styleSheets.Add(styleSheet);
            if (_config.WindowStyleSheet != null) rootVisualElement.styleSheets.Add(_config.WindowStyleSheet);
            return rootVisualElement;
        }
    }
}
