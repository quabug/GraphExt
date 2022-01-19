using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;

namespace GraphExt.GTF.Editor
{
    public class GraphModelExt<TNode, TNodeModel> : GraphModel
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeModel : NodeModelExt<TNode>
    {
        private GraphRuntime<TNode> _runtime;
        public IReadOnlyGraphRuntime<TNode> Runtime => _runtime;

        private SearcherDatabaseProviderExt _searcherDatabaseProviderExt;

        public void SetNodeSearcherItems(IEnumerable<GraphNodeModelSearcherItem> items)
        {
            _searcherDatabaseProviderExt = new SearcherDatabaseProviderExt(items);
            (Stencil as StencilExt)?.SetSearcherDatabaseProvider(_searcherDatabaseProviderExt);
        }

        [NotNull] public INodeModel CreateNode([NotNull] Type nodeType, [NotNull] IGraphNodeCreationData data)
        {
            var node = (TNode)Activator.CreateInstance(nodeType);
            var nodeId = data.Guid.ToGuid();
            _runtime.AddNode(nodeId, node);
            var nodeModel = (TNodeModel)InstantiateNode(typeof(TNodeModel), nodeType.Name, data.Position, data.Guid);;
            nodeModel.Node = node;
            AddNode(nodeModel);
            return nodeModel;
        }

        public void DeleteNodes([NotNull] IEnumerable<INodeModel> nodes)
        {
            foreach (var node in nodes) _runtime.DeleteNode(node.ToNodeId());
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            ((StencilExt)Stencil).SetSearcherDatabaseProvider(_searcherDatabaseProviderExt);

            _runtime = new GraphRuntime<TNode>();
            foreach (var node in NodeModels.OfType<NodeModelExt<TNode>>())
                _runtime.AddNode(node.Guid.ToGuid(), node.Node);
            foreach (var edge in EdgeModels.Select(edge => edge.ToEdgeId()))
                _runtime.Connect(input: edge.Input, output: edge.Output);
        }

        protected override bool IsCompatiblePort(IPortModel startPortModel, IPortModel compatiblePortModel)
        {
            var startPortId = startPortModel.ToPortId();
            var compatiblePortId = compatiblePortModel.ToPortId();
            return base.IsCompatiblePort(startPortModel, compatiblePortModel) &&
                   Runtime[startPortId.NodeId].IsPortCompatible(_runtime, input: compatiblePortId, output: startPortId) &&
                   Runtime[compatiblePortId.NodeId].IsPortCompatible(_runtime, input: compatiblePortId, output: startPortId)
            ;
        }
    }
}