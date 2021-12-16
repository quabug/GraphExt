using System;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class GraphWindow : EditorWindow
    {
        public GraphConfig Config;
        public GroupWindowExtension WindowExtension = new GroupWindowExtension();

        public void CreateGUI()
        {
            var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "GraphWindow.uxml");
            var ussPath = Path.Combine(relativeDirectory, "GraphWindow.uss");

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(rootVisualElement);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            rootVisualElement.styleSheets.Add(styleSheet);

            if (Config != null) Init(Config);
        }

        public void Init(GraphConfig config)
        {
            Config = config;
            var graph = rootVisualElement.Q<GraphView>();
            if (graph == null)
            {
                graph = new GraphView(config) { name = "graph" };
                rootVisualElement.Q<VisualElement>("graph-content").Add(graph);
            }
            var miniMap = rootVisualElement.Q<MiniMap>();
            if (miniMap != null) miniMap.graphView = graph;

            graph.Module = (IGraphModule) Activator.CreateInstance(Type.GetType(config.Backend));

            WindowExtension?.OnInitialized(this, Config, graph);
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