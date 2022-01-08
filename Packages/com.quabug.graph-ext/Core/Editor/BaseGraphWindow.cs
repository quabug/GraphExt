// using System;
// using System.IO;
// using UnityEditor;
// using UnityEditor.Experimental.GraphView;
// using UnityEngine.UIElements;
//
// namespace GraphExt.Editor
// {
//     public abstract class BaseGraphWindow : EditorWindow
//     {
//         public GroupWindowExtension WindowExtension { get; } = new GroupWindowExtension();
//         public StyleSheet StyleSheet { get; }
//
//         private readonly Lazy<VisualElement> _graphRoot;
//
//         public BaseGraphWindow()
//         {
//             _graphRoot = new Lazy<VisualElement>(LoadVisualTree);
//         }
//
//         protected virtual void CreateGUI()
//         {
//             var graphRoot = _graphRoot.Value;
//             var graph = graphRoot.Q<GraphView>();
//             if (graph == null)
//             {
//                 graph = new GraphView(config) { name = "graph" };
//                 graphRoot.Q<VisualElement>("graph-content").Add(graph);
//             }
//             var miniMap = graphRoot.Q<MiniMap>();
//             if (miniMap != null) miniMap.graphView = graph;
//
//             // WindowExtension.OnInitialized(this, Config, graph);
//         }
//
//         private VisualElement LoadVisualTree()
//         {
//             var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
//             var uxmlPath = Path.Combine(relativeDirectory, "GraphWindow.uxml");
//             var ussPath = Path.Combine(relativeDirectory, "GraphWindow.uss");
//
//             var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
//             visualTree.CloneTree(rootVisualElement);
//
//             var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
//             rootVisualElement.styleSheets.Add(styleSheet);
//             if (Config.WindowStyleSheet != null) rootVisualElement.styleSheets.Add(Config.WindowStyleSheet);
//             return rootVisualElement;
//         }
//
//         protected virtual void Update()
//         {
//             if (rootVisualElement != null) TickChildren(rootVisualElement);
//
//             void TickChildren(VisualElement parent)
//             {
//                 if (parent is ITickableElement tickable) tickable.Tick();
//                 foreach (var child in parent.Children()) TickChildren(child);
//             }
//         }
//
//         private void OnDestroy()
//         {
//             WindowExtension.OnClosed(this, Config, rootVisualElement.Q<GraphView>());
//         }
//     }
// }