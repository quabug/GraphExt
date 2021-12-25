using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class PrefabGraphBackend : BaseGraphBackend
    {
        private readonly BiDictionary<NodeId, GameObject> _nodeObjectMap = new BiDictionary<NodeId, GameObject>();
        public IReadOnlyDictionary<NodeId, GameObject> NodeObjectMap => _nodeObjectMap.Forward;
        public IReadOnlyDictionary<GameObject, NodeId> ObjectNodeMap => _nodeObjectMap.Reverse;

        private readonly HashSet<NodeId> _selectedNodes = new HashSet<NodeId>();

        public PrefabGraphBackend() {}
        public PrefabGraphBackend([NotNull] GameObject root)
        {
            foreach (var node in root.GetComponentsInChildren<INodeComponent>())
                AddNode(((Component)node).gameObject);
        }

        public override bool IsCompatible(in PortId input, in PortId output)
        {
            var inputData = _PortMap[input];
            var outputData = _PortMap[output];
            return inputData.Direction != outputData.Direction &&
                   inputData.PortType == outputData.PortType &&
                   GetNodeComponent(input.NodeId).IsPortCompatible(this, input, output) &&
                   GetNodeComponent(output.NodeId).IsPortCompatible(this, input, output)
            ;
        }

        protected override void OnConnected(in PortId input, in PortId output)
        {
            GetNodeComponent(input.NodeId).OnConnected(this, input, output);
            GetNodeComponent(output.NodeId).OnConnected(this, input, output);
        }

        protected override void OnDisconnected(in PortId input, in PortId output)
        {
            GetNodeComponent(input.NodeId).OnDisconnected(this, input, output);
            GetNodeComponent(output.NodeId).OnDisconnected(this, input, output);
        }

        public INodeComponent GetNodeComponent(in NodeId nodeId) => NodeObjectMap[nodeId].GetComponent<INodeComponent>();

        public void AddNode(GameObject nodeObject)
        {
            var node = nodeObject.GetComponent<INodeComponent>();
            _nodeObjectMap.Add(node.Id, nodeObject);
            _NodeMap.Add(node.Id, new NodeData(node.Properties.Append(CreateNodeSelector(node.Id)).ToArray()));
            foreach (var (portId, portData) in node.Ports) _PortMap.Add(portId, portData);
            foreach (var connection in node.Connections) _Connections.Add(connection);
        }

        NodeSelector CreateNodeSelector(NodeId node)
        {
            var selector = new NodeSelector();
            selector.OnSelectChanged += isSelected => OnNodeSelected(node, isSelected);
            return selector;
        }

        private void OnNodeSelected(in NodeId node, bool isSelected)
        {
#if UNITY_EDITOR
            if (isSelected)
            {
                if (_selectedNodes.Contains(node)) return;
                _selectedNodes.Add(node);
                Select(NodeObjectMap[node]);
            }
            else
            {
                if (!_selectedNodes.Contains(node)) return;
                _selectedNodes.Remove(node);
                if (_selectedNodes.Any()) Select(NodeObjectMap[_selectedNodes.First()]);
                else Select(null);
            }

            void Select(GameObject node)
            {
                if (UnityEditor.Selection.activeGameObject != node) UnityEditor.Selection.activeGameObject = node;
            }
#endif
        }
    }
}