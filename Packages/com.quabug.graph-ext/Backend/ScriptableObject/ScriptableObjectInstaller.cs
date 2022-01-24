using System;
using GraphExt.Editor;
using OneShot;
using UnityEditor;
using UnityEngine;

namespace GraphExt
{
    public class ScriptableObjectInstaller<TNode, TNodeScriptableObject> : IInstaller
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        public void Install(Container container)
        {
        }

        class SelectionObserver : IDisposable
        {
            private readonly GraphWindow _graphWindow;
            private GraphScriptableObject<TNode, TNodeScriptableObject> _graph;

            public SelectionObserver(GraphWindow graphWindow)
            {
                _graphWindow = graphWindow;
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
                if (graph == null) _graphWindow.RemoveGraphView();
                else if (_graph != graph) _graphWindow.RecreateGUI();
                _graph = graph;
            }

            private GraphScriptableObject<TNode, TNodeScriptableObject> FindCurrentGraph()
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
        }
    }
}