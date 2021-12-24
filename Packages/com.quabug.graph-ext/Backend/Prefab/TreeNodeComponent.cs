using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt.Prefab
{
    [AddComponentMenu("")]
    public class TreeNodeComponent : NodeComponent
    {
        public override IEnumerable<EdgeId> Connections => Enumerable.Empty<EdgeId>();

        protected override bool IsPortCompatible(PrefabGraphBackend graph, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            if (input.NodeId == Id) return true; // only check compatible on output/start node
            // cannot connect to input/end node which is parent of output/start node
            var inputNodeId = input.NodeId;
            return GetComponentsInParent<INodeComponent>().All(node => node.Id != inputNodeId);
        }

        protected override void OnConnected(PrefabGraphBackend graph, in PortId input, in PortId output)
        {
            if (input.NodeId == Id) transform.SetParent(graph.NodeObjectMap[output.NodeId].transform);
        }

        protected override void OnDisconnected(PrefabGraphBackend graph, in PortId input, in PortId output)
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