using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;

namespace GraphExt.GTF.Editor
{
    public class NodeModelExt<TNode> : NodeModel where TNode : INode<GraphRuntime<TNode>>
    {
        [SerializeReference] public TNode Node;

        public override void OnConnection(IPortModel selfConnectedPortModel, IPortModel otherConnectedPortModel)
        {
        }

        public override void OnDisconnection(IPortModel selfConnectedPortModel, IPortModel otherConnectedPortModel)
        {
        }
    }
}