using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public class TreeEdge
    {
        public bool IsTransforming { get; private set; } = false;

        public EdgeId? Edge(GameObject node)
        {
            var transform = node.transform;
            if (transform.parent == null) return null;
            var selfNode = node.GetComponent<ITreeNodeComponent>();
            var parentNode = transform.parent.GetComponent<ITreeNodeComponent>();
            if (parentNode == null) return null;
            return new EdgeId(selfNode.InputPort, parentNode.OutputPort);
        }

        public bool IsParentInputPort(GameObject node, PortId port) => node.GetComponentsInParent<ITreeNodeComponent>().Any(node => node.InputPort == port);
        public bool IsParentOutputPort(GameObject node, PortId port) => node.GetComponentsInParent<ITreeNodeComponent>().Any(node => node.OutputPort == port);
        public bool IsParentTreePort(GameObject node, PortId port) => node.GetComponentsInParent<ITreeNodeComponent>().Any(node => node.InputPort == port || node.OutputPort == port);

        public void ConnectParent<TComponent>(TComponent treeNode, in EdgeId edge, Transform parent)
            where TComponent : MonoBehaviour, ITreeNodeComponent
        {
            if (edge.Input == treeNode.InputPort) SetParent(parent: parent, self: treeNode.transform);
        }

        public void DisconnectParent<TComponent>(TComponent treeNode, in EdgeId edge)
            where TComponent : MonoBehaviour, ITreeNodeComponent
        {
            if (edge.Input == treeNode.InputPort)
                SetParent(parent: FindStageRoot(treeNode.transform), self: treeNode.transform);
        }

        public void OnBeforeTransformParentChanged<TComponent>(TComponent treeNode)
            where TComponent : MonoBehaviour, ITreeNodeComponent
        {
            if (IsTransforming) return;

            var edge = Edge(treeNode.gameObject);
            if (edge.HasValue)
            {
                IsTransforming = true;
                treeNode.OnNodeComponentDisconnect?.Invoke(treeNode.Id, edge.Value);
                IsTransforming = false;
            }
        }

        public void OnTransformParentChanged<TComponent>(TComponent treeNode)
            where TComponent : MonoBehaviour, ITreeNodeComponent
        {
            if (IsTransforming) return;

            var edge = Edge(treeNode.gameObject);
            if (edge.HasValue)
            {
                IsTransforming = true;
                treeNode.OnNodeComponentConnect?.Invoke(treeNode.Id, edge.Value);
                IsTransforming = false;
            }
        }

        private Transform FindStageRoot(Transform transform)
        {
            var self = transform;
            while (self.parent != null) self = self.parent;
            return self;
        }

        private void SetParent(Transform self, Transform parent)
        {
            if (!IsTransforming)
            {
                IsTransforming = true;
                self.SetParent(parent);
                IsTransforming = false;
            }
        }
    }
}