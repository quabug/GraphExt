using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class PrefabBackend : BaseGraphBackend
    {
        private GameObject _root { get; }

        public PrefabBackend() {}

        public PrefabBackend([NotNull] GameObject root) :base()
        {
            _root = root;
            Selection.selectionChanged += OnSelectionChanged;
        }

        void OnSelectionChanged()
        {
            var selectedInstance = Selection.activeGameObject;
            if (selectedInstance != null)
            {
            }
        }

        public override bool IsCompatible(in PortId input, in PortId output)
        {
            var inputData = _PortMap[input];
            var outputData = _PortMap[output];
            return inputData.Direction != outputData.Direction &&
                   inputData.PortType == outputData.PortType;
        }

        protected override void OnConnected(in PortId input, in PortId output)
        {
            // GetMemoryNodeByPort(input).OnConnected(this, output.Id, input.Id);
            // GetMemoryNodeByPort(output).OnConnected(this, output.Id, input.Id);
        }

        protected override void OnDisconnected(in PortId input, in PortId output)
        {
            // GetMemoryNodeByPort(input).OnDisconnected(this, output.Id, input.Id);
            // GetMemoryNodeByPort(output).OnDisconnected(this, output.Id, input.Id);
        }

        public GameObjectNode CreateNode(IGameObjectNode node)
        {
            var obj = new GameObject();
            var objNode = obj.AddComponent<GameObjectNode>();
            objNode.Node = node;
            return objNode;
        }
    }
}