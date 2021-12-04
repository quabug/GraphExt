using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class GraphWindow : EditorWindow
    {
        private Graph _view => rootVisualElement.Q<Graph>();

        public void CreateGUI()
        {
            var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "GraphWindow.uxml");
            var ussPath = Path.Combine(relativeDirectory, "GraphWindow.uss");

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(rootVisualElement);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            rootVisualElement.styleSheets.Add(styleSheet);

            var miniMap = rootVisualElement.Q<MiniMap>();
            var graph = _view;
            if (miniMap != null && graph != null) miniMap.graphView = graph;

            // ResetEditorView();
        }

        private void Reset()
        {
            // BehaviorTreeGraph graph = null;
            // if (prefabStage != null)
            // {
            //     var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.assetPath);
            //     graph = new BehaviorTreeGraph(prefab, prefabStage);
            // }
            // rootVisualElement.Q<Graph>().Reset(graph);
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
    }
}