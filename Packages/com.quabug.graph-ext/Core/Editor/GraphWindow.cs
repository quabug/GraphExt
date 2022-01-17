using System;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public abstract class GraphWindow : EditorWindow
    {
        public GraphConfig Config;

        private readonly Lazy<VisualElement> _graphRoot;

        protected GraphWindow()
        {
            _graphRoot = new Lazy<VisualElement>(LoadVisualTree);
        }

        private void CreateGUI()
        {
            if (Config != null) Init(Config);
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
            if (Config.WindowStyleSheet != null) rootVisualElement.styleSheets.Add(Config.WindowStyleSheet);
            return rootVisualElement;
        }

        public void Init([NotNull] GraphConfig config)
        {
            Config = config;

            var graphRoot = _graphRoot.Value;
            var graph = graphRoot.Q<GraphView>();
            if (graph == null)
            {
                graph = CreateGraphView();
                graphRoot.Q<VisualElement>("graph-content").Add(graph);
            }
            var miniMap = graphRoot.Q<MiniMap>();
            if (miniMap != null) miniMap.graphView = graph;
        }

        // TODO: optimize
        private void Update()
        {
            if (rootVisualElement != null) TickChildren(rootVisualElement);

            void TickChildren(VisualElement parent)
            {
                if (parent is ITickableElement tickable) tickable.Tick();
                foreach (var child in parent.Children()) TickChildren(child);
            }
        }

        protected abstract GraphView CreateGraphView();
    }
}