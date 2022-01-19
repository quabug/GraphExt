using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine;

namespace GraphExt.GTF.Editor
{
    public interface INodeSearcherItems
    {
        IEnumerable<GraphNodeModelSearcherItem> GetItems([NotNull] IGraphModel graphModel);
    }

    public class StickyNoteItem : INodeSearcherItems
    {
        public IEnumerable<GraphNodeModelSearcherItem> GetItems(IGraphModel graphModel)
        {
            var item = new GraphNodeModelSearcherItem(
                graphModel,
                new TagSearcherItemData(CommonSearcherTags.StickyNote),
                data =>
                {
                    var rect = new Rect(data.Position, StickyNote.defaultSize);
                    return data.GraphModel.CreateStickyNote(rect, data.SpawnFlags);
                },
                "Sticky Note"
            );
            yield return item;
        }
    }

    public class NodeItems<TNode, TNodeModel> : INodeSearcherItems
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeModel : NodeModelExt<TNode>, new()
    {
        public IEnumerable<GraphNodeModelSearcherItem> GetItems(IGraphModel graphModel)
        {
            var nodeTypes = TypeCache.GetTypesDerivedFrom<TNode>();
            foreach (var type in nodeTypes
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .OrderBy(type => type.Name)
            )
            {
                yield return new GraphNodeModelSearcherItem(
                    graphModel,
                    new NodeSearcherItemData(type),
                    data => data.SpawnFlags.IsOrphan() ?
                        data.CreateNode(typeof(TNodeModel), type.Name) :
                        ((GraphModelExt<TNode, TNodeModel>)data.GraphModel).CreateNode(type, data),
                    type.Name
                );
            }
        }
    }
}