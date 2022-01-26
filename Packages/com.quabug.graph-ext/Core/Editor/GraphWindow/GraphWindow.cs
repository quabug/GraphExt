using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public sealed class GraphWindow : EditorWindow
    {
        [SerializeField] private GraphConfig _config;

        public void SetGraphConfig(GraphConfig config)
        {
            if (_config != config)
            {
                _config = config;
                CreateGUI();
            }
        }

        private readonly Lazy<VisualElement> _graphRoot;

        public GraphWindow()
        {
            _graphRoot = new Lazy<VisualElement>(LoadVisualTree);
        }

        public void CreateGUI()
        {
            if (_config)
            {
                _config.GraphWindowExtension.Clear();
                var graphViewRoot = _graphRoot.Value.Q<VisualElement>("graph-content");
                _config.GraphWindowExtension.Recreate(graphViewRoot);
            }
        }

        private void Update()
        {
            if (_config) _config.GraphWindowExtension.Tick();
        }

        private void OnDestroy()
        {
            if (_config) _config.GraphWindowExtension.Clear();
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
