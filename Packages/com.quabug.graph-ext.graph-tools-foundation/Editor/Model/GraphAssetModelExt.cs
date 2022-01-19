using System;
using System.Linq;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;

namespace GraphExt.GTF.Editor
{
    public abstract class GraphAssetModelExt<TNode, TGraphModel, TNodeModel> : GraphAssetModel
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeModel : NodeModelExt<TNode>, new()
        where TGraphModel : GraphModelExt<TNode, TNodeModel>
    {
        protected override Type GraphModelType => typeof(TGraphModel);

        [SerializeReference, SerializeReferenceDrawer] public INodeSearcherItems[] NodeSearcherItems;

        protected override void OnEnable()
        {
            base.OnEnable();
            ((TGraphModel)GraphModel).SetNodeSearcherItems(NodeSearcherItems.SelectMany(items => items.GetItems(GraphModel)));
        }

        public static void CreateGraph<TAsset, TWindow>()
            where TWindow : GraphWindowExt<TNode, TGraphModel, TNodeModel>
            where TAsset : GraphAssetModelExt<TNode, TGraphModel, TNodeModel>
        {
            const string path = "Assets";
            var template = new GraphTemplate<StencilExt>();
            CommandDispatcher commandDispatcher = null;
            if (EditorWindow.HasOpenInstances<TWindow>())
            {
                var window = EditorWindow.GetWindow<TWindow>();
                if (window != null)
                {
                    commandDispatcher = window.CommandDispatcher;
                }
            }

            GraphAssetCreationHelpers<TAsset>.CreateInProjectWindow(template, commandDispatcher, path);
        }

        public static bool OpenGraphAsset<TAsset, TWindow>(int instanceId, int line)
            where TWindow : GraphWindowExt<TNode, TGraphModel, TNodeModel>
            where TAsset : GraphAssetModelExt<TNode, TGraphModel, TNodeModel>
        {
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            if (obj is TAsset graphAssetModel)
            {
                var window = GraphViewEditorWindow.FindOrCreateGraphWindow<TWindow>();
                window.SetCurrentSelection(graphAssetModel, GraphViewEditorWindow.OpenMode.OpenAndFocus);
                return window != null;
            }

            return false;
        }
    }
}