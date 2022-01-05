#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    public sealed class GameObjectHierarchyGraphViewModule<TNode, TComponent> : GraphViewModule<TNode>, IDisposable
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        [NotNull] public GameObjectNodes<TNode, TComponent> GameObjectNodes { get; }
        [NotNull] public override GraphRuntime<TNode> Runtime => GameObjectNodes.Graph;

        public GameObjectHierarchyGraphViewModule()
        {
            GameObjectNodes = new GameObjectNodes<TNode, TComponent>();
        }

        public GameObjectHierarchyGraphViewModule(GameObject root)
        {
            GameObjectNodes = new GameObjectNodes<TNode, TComponent>(root);
            foreach (var nodeId in Runtime.NodeMap.Keys)
            {
                _PortData[nodeId] = FindNodePorts(nodeId);
                _NodeData[nodeId] = ToNodeData(nodeId);
            }
            Utility.SavePrefabStage();
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        public void Dispose()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            GameObjectNodes.Dispose();
        }

        private void OnHierarchyChanged()
        {
            GameObjectNodes.Refresh();
        }

        public void AddGameObjectNode(in NodeId nodeId, TNode node, Vector2 position)
        {
            AddNode(nodeId, node, position.x, position.y);
            Utility.SavePrefabStage();
        }

        public override bool IsCompatible(in PortId input, in PortId output)
        {
            return base.IsCompatible(in input, in output) && GameObjectNodes.IsPortCompatible(input, output);
        }

        public override void DeleteNode(in NodeId nodeId)
        {
            base.DeleteNode(in nodeId);
            Utility.SavePrefabStage();
        }

        public override void Connect(in PortId input, in PortId output)
        {
            base.Connect(in input, in output);
            Utility.SavePrefabStage();
        }

        public override void Disconnect(in PortId input, in PortId output)
        {
            base.Disconnect(in input, in output);
            Utility.SavePrefabStage();
        }

        public override void SetNodePosition(in NodeId nodeId, float x, float y)
        {
            GameObjectNodes.SetPosition(nodeId, new Vector2(x, y));
            Utility.SavePrefabStage();
        }

        protected override IReadOnlyDictionary<string, PortData> FindNodePorts(in NodeId nodeId)
        {
            return GameObjectNodes[nodeId].GetComponent<TComponent>().FindNodePorts(GameObjectNodes);
        }

        protected override NodeData ToNodeData(in NodeId nodeId)
        {
            return GameObjectNodes[nodeId].GetComponent<TComponent>().FindNodeProperties(GameObjectNodes);
        }
    }
}

#endif