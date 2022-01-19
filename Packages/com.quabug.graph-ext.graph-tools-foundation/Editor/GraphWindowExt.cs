using UnityEditor.GraphToolsFoundation.Overdrive;

namespace GraphExt.GTF.Editor
{
    public class GraphWindowExt<TNode, TGraphModel, TNodeModel> : GraphViewEditorWindow
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeModel : NodeModelExt<TNode>, new()
        where TGraphModel : GraphModelExt<TNode, TNodeModel>
    {
        protected virtual string _EditorToolName => "Graph";

        protected override void OnEnable()
        {
            EditorToolName = _EditorToolName;
            base.OnEnable();
        }

        // /// <inheritdoc />
        // protected override GraphToolState CreateInitialState()
        // {
        //     var prefs = Preferences.CreatePreferences(EditorToolName);
        //     return new RecipeState(GUID, prefs);
        // }
        //
        protected override GraphView CreateGraphView()
        {
            return new GraphViewExt<TNode, TNodeModel>(this, CommandDispatcher, EditorToolName);
        }
        //
        // protected override BlankPage CreateBlankPage()
        // {
        //     var onboardingProviders = new List<OnboardingProvider>();
        //     onboardingProviders.Add(new RecipeOnboardingProvider());
        //
        //     return new BlankPage(CommandDispatcher, onboardingProviders);
        // }

        protected override bool CanHandleAssetType(IGraphAssetModel asset)
        {
            return asset is GraphAssetModelExt<TNode, TGraphModel, TNodeModel>;
        }
    }
}