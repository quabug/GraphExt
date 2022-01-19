using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Searcher;

namespace GraphExt.GTF.Editor
{
    public class SearcherDatabaseProviderExt : ISearcherDatabaseProvider
    {
        private readonly List<SearcherDatabaseBase> _graphElementsSearcherDatabases;

        public SearcherDatabaseProviderExt([NotNull, ItemNotNull] IEnumerable<GraphNodeModelSearcherItem> nodeItems)
        {
            _graphElementsSearcherDatabases = new List<SearcherDatabaseBase> {
                new SearcherDatabase(nodeItems.OfType<SearcherItem>().ToList())
            };
        }

        public virtual List<SearcherDatabaseBase> GetGraphElementsSearcherDatabases(IGraphModel graphModel)
        {
            return _graphElementsSearcherDatabases;
        }

        public virtual List<SearcherDatabaseBase> GetVariableTypesSearcherDatabases()
        {
            return new List<SearcherDatabaseBase>();
        }

        public virtual List<SearcherDatabaseBase> GetGraphVariablesSearcherDatabases(IGraphModel graphModel)
        {
            return new List<SearcherDatabaseBase>();
        }

        public virtual List<SearcherDatabaseBase> GetDynamicSearcherDatabases(IPortModel portModel)
        {
            return new List<SearcherDatabaseBase>();
        }

        public virtual List<SearcherDatabaseBase> GetDynamicSearcherDatabases(IEnumerable<IPortModel> portModel)
        {
            return new List<SearcherDatabaseBase>();
        }
    }
}