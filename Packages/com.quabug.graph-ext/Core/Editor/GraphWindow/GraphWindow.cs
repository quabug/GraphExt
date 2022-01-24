using System;
using System.IO;
using System.Linq;
using OneShot;
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
                    RecreateGUI();
                }
            }
        }

        private readonly Lazy<VisualElement> _graphRoot;
        private Container _container;
        private MenuBuilder _menuBuilder;
        private IViewPresenter[] _presenters = Array.Empty<IViewPresenter>();

        public GraphWindow()
        {
            _graphRoot = new Lazy<VisualElement>(LoadVisualTree);
        }

        public void CreateGUI()
        {
            if (_config) RecreateGUI();
        }

        private void Update()
        {
            foreach (var presenter in _presenters.OfType<ITickablePresenter>()) presenter.Tick();
        }

        public void RecreateGUI()
        {
            _container?.Dispose();
            foreach (var presenter in _presenters.OfType<IDisposable>()) presenter.Dispose();
            _container = new Container();
            foreach (var installer in _config.Installers) installer.Install(_container);
            foreach (var installer in _config.MenuEntries) installer.Install(_container);
            var graphView = _container.Resolve<UnityEditor.Experimental.GraphView.GraphView>();
            ReplaceGraphView(graphView);
            _presenters = _container.ResolveGroup<IViewPresenter>().ToArray();
            _menuBuilder = new MenuBuilder(graphView, _container.ResolveGroup<IMenuEntry>().Reverse().ToArray());
        }

        private void RemoveGraphView()
        {
            var graph = _graphRoot.Value.Q<UnityEditor.Experimental.GraphView.GraphView>();
            graph?.parent.Remove(graph);
        }

        private void AddGraphView(UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            var graphRoot = _graphRoot.Value;
            graphRoot.Q<VisualElement>("graph-content").Add(graphView);
            var miniMap = graphRoot.Q<MiniMap>();
            if (miniMap != null) miniMap.graphView = graphView;
        }

        private void ReplaceGraphView(UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            RemoveGraphView();
            AddGraphView(graphView);
        }

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
