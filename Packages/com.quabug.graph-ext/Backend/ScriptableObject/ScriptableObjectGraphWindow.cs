using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphWindow<TNode, TNodeScriptableObject> : BaseGraphWindow
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        protected ScriptableObjectGraphSetup<TNode, TNodeScriptableObject> _GraphSetup;

        protected override void CreateGUI()
        {
            OnSelectionChanged();
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
        }

        protected virtual void Update()
        {
            _GraphSetup?.Tick();
        }

        protected virtual void OnDestroy()
        {
            _GraphSetup?.Dispose();
            Selection.selectionChanged -= OnSelectionChanged;
        }

        protected void OnSelectionChanged()
        {
            var graph = FindCurrentGraph();
            if (graph == null)
            {
                RemoveGraphView();
                _GraphSetup?.Dispose();
                _GraphSetup = null;
                OnGraphRecreated();
            }
            else if (_GraphSetup == null || graph != _GraphSetup.Graph)
            {
                _GraphSetup?.Dispose();
                _GraphSetup = new ScriptableObjectGraphSetup<TNode, TNodeScriptableObject>(graph);
                ReplaceGraphView(_GraphSetup.GraphView);
                OnGraphRecreated();
            }
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

        protected virtual void OnGraphRecreated() {}
    }
}