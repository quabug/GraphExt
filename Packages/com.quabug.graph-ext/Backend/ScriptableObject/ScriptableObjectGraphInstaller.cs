using System;
using System.Collections.Generic;
using GraphExt.Editor;
using OneShot;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using GraphView = UnityEditor.Experimental.GraphView.GraphView;

namespace GraphExt
{
    public class ScriptableObjectGraphInstaller<TNode, TNodeScriptableObject> : IGraphInstaller
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        public void Install(Container container)
        {
            var graph = FindCurrentGraph();
            if (graph != null)
            {
                container.RegisterInstance(graph);
                container.RegisterGraphRuntimeInstance(graph.Runtime);
                container.RegisterSingleton(() =>
                {
                    var graphView = container.Resolve<GraphView>();
                    var nodeViews = container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                    return new FocusActiveNodePresenter<TNodeScriptableObject>(
                        graphView,
                        node => nodeViews[graph[node]],
                        () => Selection.activeObject as TNodeScriptableObject
                    );
                });
                container.Register<IWindowSystem>(container.Resolve<FocusActiveNodePresenter<TNodeScriptableObject>>);
                container.RegisterSingleton(() =>
                {
                    var nodes = container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                    var nodeObjects = container.Resolve<IReadOnlyDictionary<NodeId, TNodeScriptableObject>>();
                    return new ActiveSelectedNodePresenter<TNodeScriptableObject>(nodes, nodeObjects, node =>
                    {
                        if (Selection.activeObject != node) Selection.activeObject = node;
                    });
                });
                container.Register<IWindowSystem>(container.Resolve<ActiveSelectedNodePresenter<TNodeScriptableObject>>);
            }
        }

        private static GraphScriptableObject<TNode, TNodeScriptableObject> FindCurrentGraph()
        {
            switch (Selection.activeObject)
            {
                case GraphScriptableObject<TNode, TNodeScriptableObject> graph:
                    return graph;
                case ScriptableObject obj:
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    return AssetDatabase.LoadAssetAtPath<GraphScriptableObject<TNode, TNodeScriptableObject>>(path);
                }
                default:
                    return null;
            }
        }

        class SelectionObserver : IDisposable
        {
            private readonly GraphView _graphView;
            private GraphScriptableObject<TNode, TNodeScriptableObject> _graph;

            public SelectionObserver(GraphView graphView)
            {
                _graphView = graphView;
                OnSelectionChanged();
                Selection.selectionChanged -= OnSelectionChanged;
                Selection.selectionChanged += OnSelectionChanged;
            }

            public void Dispose()
            {
                Selection.selectionChanged -= OnSelectionChanged;
            }

            protected void OnSelectionChanged()
            {
                var graph = FindCurrentGraph();
                // if (graph == null) _graphView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                // if (_graph != graph) _graphWindow.RecreateGUI();
                _graph = graph;
            }
        }
    }
}