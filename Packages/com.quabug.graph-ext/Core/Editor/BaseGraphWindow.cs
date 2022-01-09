using System;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public abstract class BaseGraphWindow : EditorWindow
    {
        public GroupWindowExtension WindowExtension { get; } = new GroupWindowExtension();
        private readonly Lazy<VisualElement> _graphRoot;
        protected abstract UnityEditor.Experimental.GraphView.GraphView _GraphView { get; }

        public BaseGraphWindow()
        {
            _graphRoot = new Lazy<VisualElement>(LoadVisualTree);
        }

        protected static void OpenWindow<TGraphWindow>(string windowName) where TGraphWindow : BaseGraphWindow
        {
            var window = Window.GetOrCreate<TGraphWindow>(windowName);
            window.titleContent.text = windowName;
            window.Focus();
        }

        protected virtual void CreateGUI()
        {
            var graphRoot = _graphRoot.Value;
            var graph = graphRoot.Q<UnityEditor.Experimental.GraphView.GraphView>();
            if (graph != null) graphRoot.Remove(graph);
            graph = _GraphView;
            graphRoot.Q<VisualElement>("graph-content").Add(graph);
            var miniMap = graphRoot.Q<MiniMap>();
            if (miniMap != null) miniMap.graphView = graph;

            // WindowExtension.OnInitialized(this, Config, graph);
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
            return rootVisualElement;
        }

        protected virtual void Update() {}

        protected virtual void OnDestroy()
        {
            // WindowExtension.OnClosed(this, Config, rootVisualElement.Q<GraphView>());
        }
    }
}