using System;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public abstract class BaseGraphWindow : EditorWindow
    {
        private readonly Lazy<VisualElement> _graphRoot;

        protected BaseGraphWindow()
        {
            _graphRoot = new Lazy<VisualElement>(LoadVisualTree);
        }

        protected static void OpenWindow<TGraphWindow>(string windowName) where TGraphWindow : BaseGraphWindow
        {
            var window = Window.GetOrCreate<TGraphWindow>(windowName);
            window.titleContent.text = windowName;
            window.Show(immediateDisplay: true);
            window.Focus();
        }

        protected abstract void CreateGUI();

        protected void RemoveGraphView()
        {
            var graph = _graphRoot.Value.Q<UnityEditor.Experimental.GraphView.GraphView>();
            graph?.parent.Remove(graph);
        }

        protected void AddGraphView(UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            var graphRoot = _graphRoot.Value;
            graphRoot.Q<VisualElement>("graph-content").Add(graphView);
            var miniMap = graphRoot.Q<MiniMap>();
            if (miniMap != null) miniMap.graphView = graphView;
        }

        protected void ReplaceGraphView(UnityEditor.Experimental.GraphView.GraphView graphView)
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
            return rootVisualElement;
        }
    }
}