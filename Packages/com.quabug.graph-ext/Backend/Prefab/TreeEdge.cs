using System.Linq;
using UnityEngine;

namespace GraphExt
{
    [RequireComponent(typeof(ITreeNodeComponent)), AddComponentMenu(""), ExecuteAlways, DisallowMultipleComponent]
    public class TreeEdge : MonoBehaviour
    {
        public bool IsTransforming { get; private set; } = false;

        private ITreeNodeComponent _Node => GetComponent<ITreeNodeComponent>();

        public EdgeId? Edge
        {
            get
            {
                if (transform.parent == null) return null;
                var selfNode = GetComponent<ITreeNodeComponent>();
                var parentNode = transform.parent.GetComponent<ITreeNodeComponent>();
                if (parentNode == null) return null;
                return new EdgeId(selfNode.InputPort, parentNode.OutputPort);
            }
        }

        public bool IsParentInputPort(PortId port) => GetComponentsInParent<ITreeNodeComponent>().Any(node => node.InputPort == port);
        public bool IsParentOutputPort(PortId port) => GetComponentsInParent<ITreeNodeComponent>().Any(node => node.OutputPort == port);
        public bool IsParentTreePort(PortId port) => GetComponentsInParent<ITreeNodeComponent>().Any(node => node.InputPort == port || node.OutputPort == port);

        public void ConnectParent(in EdgeId edge, Transform parent)
        {
            if (edge.Input == _Node.InputPort) SetParent(parent);
        }

        public void DisconnectParent(in EdgeId edge)
        {
            if (edge.Input == _Node.InputPort) SetParent(FindStageRoot());
        }

        private void OnBeforeTransformParentChanged()
        {
            if (IsTransforming) return;

            var edge = Edge;
            if (edge.HasValue)
            {
                IsTransforming = true;
                _Node.OnNodeComponentDisconnect?.Invoke(_Node.Id, edge.Value);
                IsTransforming = false;
            }
        }

        private void OnTransformParentChanged()
        {
            if (IsTransforming) return;

            var edge = Edge;
            if (edge.HasValue)
            {
                IsTransforming = true;
                _Node.OnNodeComponentConnect?.Invoke(_Node.Id, edge.Value);
                IsTransforming = false;
            }
        }

        private Transform FindStageRoot()
        {
            var self = transform;
            while (self.parent != null) self = self.parent;
            return self;
        }

        private void SetParent(Transform parent)
        {
            if (!IsTransforming)
            {
                IsTransforming = true;
                transform.SetParent(parent);
                IsTransforming = false;
            }
        }
    }
}