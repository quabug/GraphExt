using System.Linq;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace GraphExt.GTF.Editor
{
    public class GraphViewExt<TNode, TNodeModel> : GraphView
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeModel : NodeModelExt<TNode>
    {
        public GraphViewExt(GraphViewEditorWindow window, CommandDispatcher commandDispatcher, string graphViewName) : base(window, commandDispatcher, graphViewName)
        {
            commandDispatcher.RegisterCommandHandler<DeleteElementsCommand>(DeleteElementsHandler);
        }

        public static void DeleteElementsHandler(GraphToolState graphToolState, DeleteElementsCommand command)
        {
            if (!command.Models.Any())
                return;

            graphToolState.PushUndo(command);

            using var selectionUpdater = graphToolState.SelectionState.UpdateScope;
            using var graphUpdater = graphToolState.GraphViewState.UpdateScope;
            var deletedModels = graphToolState.GraphViewState.GraphModel.DeleteElements(command.Models).ToList();
            (graphToolState.GraphViewState.GraphModel as GraphModelExt<TNode, TNodeModel>)?.DeleteNodes(deletedModels.OfType<INodeModel>());

            var selectedModels = deletedModels.Where(m => graphToolState.SelectionState.IsSelected(m)).ToList();
            if (selectedModels.Any())
            {
                selectionUpdater.SelectElements(selectedModels, false);
            }

            graphUpdater.MarkDeleted(deletedModels);
        }
    }
}