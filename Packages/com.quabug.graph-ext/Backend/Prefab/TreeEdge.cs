using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public class TreeEdge
    {
        private GameObject _TreeNodeObject => ((Component)_treeNode).gameObject;
        private readonly ITreeNodeComponent _treeNode;
        public bool IsTransforming { get; private set; } = false;

        public EdgeId? Edge
        {
            get
            {
                var transform = _TreeNodeObject.transform;
                if (transform.parent == null) return null;
                var selfNode = _TreeNodeObject.GetComponent<ITreeNodeComponent>();
                var parentNode = transform.parent.GetComponent<ITreeNodeComponent>();
                if (parentNode == null) return null;
                return new EdgeId(selfNode.InputPort, parentNode.OutputPort);
            }
        }

        public TreeEdge([NotNull] ITreeNodeComponent treeNode)
        {
            _treeNode = treeNode;
        }

        public bool IsParentInputPort(PortId port) => _TreeNodeObject.GetComponentsInParent<ITreeNodeComponent>().Any(node => node.InputPort == port);
        public bool IsParentOutputPort(PortId port) => _TreeNodeObject.GetComponentsInParent<ITreeNodeComponent>().Any(node => node.OutputPort == port);
        public bool IsParentTreePort(PortId port) => _TreeNodeObject.GetComponentsInParent<ITreeNodeComponent>().Any(node => node.InputPort == port || node.OutputPort == port);

        public void ConnectParent(in EdgeId edge, Transform parent)
        {
            if (edge.Input == _treeNode.InputPort) SetParent(parent);
        }

        public void DisconnectParent(in EdgeId edge)
        {
            if (edge.Input == _treeNode.InputPort) SetParent(FindStageRoot());
        }

        public void OnBeforeTransformParentChanged()
        {
            if (IsTransforming) return;

            var edge = Edge;
            if (edge.HasValue)
            {
                IsTransforming = true;
                _treeNode.OnNodeComponentDisconnect?.Invoke(_treeNode.Id, edge.Value);
                IsTransforming = false;
            }
        }

        public void OnTransformParentChanged()
        {
            if (IsTransforming) return;

            var edge = Edge;
            if (edge.HasValue)
            {
                IsTransforming = true;
                _treeNode.OnNodeComponentConnect?.Invoke(_treeNode.Id, edge.Value);
                IsTransforming = false;
            }
        }

        private Transform FindStageRoot()
        {
            var self = _TreeNodeObject.transform;
            while (self.parent != null) self = self.parent;
            return self;
        }

        private void SetParent(Transform parent)
        {
            if (!IsTransforming)
            {
                IsTransforming = true;
                _TreeNodeObject.transform.SetParent(parent);
                IsTransforming = false;
            }
        }
    }
}