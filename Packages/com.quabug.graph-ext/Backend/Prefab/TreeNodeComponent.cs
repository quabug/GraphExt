using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt
{
    public interface ITreeNode<in TGraph> : INode<TGraph>
    {
        string InputPortName { get; }
        string OutputPortName { get; }
    }

    [AddComponentMenu("")]
    public class TreeNodeComponent<TNode> : NodeComponent<TNode> where TNode : ITreeNode<GraphRuntime<TNode>>
    {
        public override IReadOnlySet<EdgeId> Edges
        {
            get
            {
                var parentNode = transform.parent.GetComponent<TreeNodeComponent<TNode>>();
                var edges = new HashSet<EdgeId>();
                if (parentNode != null) edges.Add(new EdgeId(InputPort, parentNode.OutputPort));
                return edges;
            }
        }

        public PortId InputPort => new PortId(Id, Node.InputPortName);
        public PortId OutputPort => new PortId(Id, Node.OutputPortName);

        protected override bool IsPortCompatible(GameObjectNodes<TNode> graph, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            if (input.NodeId == Id) return true; // only check compatible on output/start node
            // cannot connect to input/end node which is parent of output/start node
            var inputNodeId = input.NodeId;
            return GetComponentsInParent<INodeComponent<TNode>>().All(node => node.Id != inputNodeId);
        }

        protected override void OnConnected(GameObjectNodes<TNode> graph, in PortId input, in PortId output)
        {
            if (input.NodeId == Id) transform.SetParent(graph[output.NodeId].transform);
        }

        protected override void OnDisconnected(GameObjectNodes<TNode> graph, in PortId input, in PortId output)
        {
            if (input.NodeId == Id) transform.SetParent(FindStageRoot());
        }

        private Transform FindStageRoot()
        {
            var self = transform;
            while (self.parent != null) self = self.parent;
            return self;
        }
    }
}