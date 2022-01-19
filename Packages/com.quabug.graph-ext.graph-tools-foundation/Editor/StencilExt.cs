using UnityEditor.GraphToolsFoundation.Overdrive;

namespace GraphExt.GTF.Editor
{
    public class StencilExt : Stencil
    {
        public override string ToolName => "Graph";

        // ReSharper disable once EmptyConstructor
        public StencilExt() { }

        public override IBlackboardGraphModel CreateBlackboardGraphModel(IGraphAssetModel graphAssetModel)
        {
            return new BlackboardGraphModelExt(graphAssetModel);
        }

        public override ISearcherDatabaseProvider GetSearcherDatabaseProvider()
        {
            return m_SearcherDatabaseProvider ?? new DefaultSearcherDatabaseProvider(this);
        }

        public void SetSearcherDatabaseProvider(ISearcherDatabaseProvider provider) => m_SearcherDatabaseProvider = provider;
    }
}