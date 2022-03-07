#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphWindowExtension<TNode, TNodeScriptableObject> : BaseGraphWindowExtension
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        private GraphScriptableObject<TNode, TNodeScriptableObject> _graph;

        protected override void Recreate()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
            OnSelectionChanged();
        }

        private void OnSelectionChanged()
        {
            var graph = FindCurrentGraph();
            if (graph == null)
            {
                RemoveGraphView();
                Clear();
            }
            else if (_graph != graph)
            {
                _Container = new Container();
                graph.Initialize();
                _Container.RegisterInstance(graph).AsSelf().As<ScriptableObject>();
                _Container.RegisterSerializableGraphBackend(graph);
                _Container.Register<Func<NodeId, NodeScriptableObject>>((resolveContainer, contractType) =>
                {
                    var nodes = _Container.Resolve<IReadOnlyDictionary<NodeId, TNodeScriptableObject>>();
                    return id => nodes[id];
                }).AsSelf();
                base.Recreate();
            }
            _graph = graph;
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
    }
}

#endif