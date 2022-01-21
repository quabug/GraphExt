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
        [SerializeField] private GraphConfig _Config;
        public GraphConfig Config
        {
            set
            {
                if (_Config != value)
                {
                    _Config = value;
                    // RecreateGUI();
                }
            }
        }

        private readonly Lazy<VisualElement> _graphRoot;

        public GraphWindow()
        {
            _graphRoot = new Lazy<VisualElement>(LoadVisualTree);
        }

        public void CreateGUI()
        {
            // if (_Config) RecreateGUI();
        }

        // protected abstract void RecreateGUI();

        public void RemoveGraphView()
        {
            var graph = _graphRoot.Value.Q<UnityEditor.Experimental.GraphView.GraphView>();
            graph?.parent.Remove(graph);
        }

        public void AddGraphView(UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            var graphRoot = _graphRoot.Value;
            graphRoot.Q<VisualElement>("graph-content").Add(graphView);
            var miniMap = graphRoot.Q<MiniMap>();
            if (miniMap != null) miniMap.graphView = graphView;
        }

        public void ReplaceGraphView(UnityEditor.Experimental.GraphView.GraphView graphView)
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
            if (_Config.WindowStyleSheet != null) rootVisualElement.styleSheets.Add(_Config.WindowStyleSheet);
            return rootVisualElement;
        }
    }
}
