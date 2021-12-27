using System;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class GraphWindow : EditorWindow
    {
        public GraphConfig Config;
        public GroupWindowExtension WindowExtension = new GroupWindowExtension();
        private bool _isInitialized = false;

        public void CreateGUI()
        {
            if (!_isInitialized && Config != null) Init(Config);
        }

        private void LoadVisualTree()
        {
            var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "GraphWindow.uxml");
            var ussPath = Path.Combine(relativeDirectory, "GraphWindow.uss");

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(rootVisualElement);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        public void Init([NotNull] GraphConfig config)
        {
            _isInitialized = true;

            Config = config;
            LoadVisualTree();

            var graph = rootVisualElement.Q<GraphView>();
            if (graph == null)
            {
                graph = new GraphView(config) { name = "graph" };
                rootVisualElement.Q<VisualElement>("graph-content").Add(graph);
            }
            var miniMap = rootVisualElement.Q<MiniMap>();
            if (miniMap != null) miniMap.graphView = graph;

            foreach (var windowExtensionType in config.WindowExtensions)
            {
                var type = Type.GetType(windowExtensionType);
                WindowExtension.GetOrCreate(type);
            }
            WindowExtension.OnInitialized(this, Config, graph);
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

        private void OnDestroy()
        {
            WindowExtension.OnClosed(this, Config, rootVisualElement.Q<GraphView>());
        }
    }
}