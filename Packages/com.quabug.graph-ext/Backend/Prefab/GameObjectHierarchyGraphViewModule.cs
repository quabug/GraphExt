﻿#if UNITY_EDITOR

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
            foreach (var nodePair in Runtime.NodeMap)
            {
                var nodeId = nodePair.Key;
                var node = nodePair.Value;
                var ports = FindNodePorts(node);
                foreach (var port in ports) _PortData[new PortId(nodeId, port.Name)] = port;
                _NodeData[nodeId] = ToNodeData(nodeId, node);
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

        protected override IEnumerable<PortData> FindNodePorts(TNode node)
        {
            return node.FindPorts();
        }

        protected override NodeData ToNodeData(in NodeId id, TNode node)
        {
            var nodeObject = GameObjectNodes[id];
            var nodeComponent = nodeObject.GetComponent<TComponent>();
            var nodeSerializedProperty = new SerializedObject(nodeComponent).FindProperty(nodeComponent.NodeSerializedPropertyName);
            return new NodeData(new NodePositionProperty(nodeComponent.Position.x, nodeComponent.Position.y).Yield()
                .Append<INodeProperty>(CreateTitleProperty())
                .Concat(NodePropertyUtility.CreateProperties(node, id, nodeSerializedProperty))
                .ToArray()
            );

            DynamicTitleProperty CreateTitleProperty()
            {
                return new DynamicTitleProperty(() =>
                {
                    if (nodeObject == null) return "*** deleted ***";
                    var titleComponent = nodeObject.GetComponent<NodeTitle>();
                    if (titleComponent == null) return nodeObject.name;
                    return titleComponent.Type switch
                    {
                        NodeTitle.TitleType.Hidden => null,
                        NodeTitle.TitleType.GameObjectName => nodeObject.name,
                        NodeTitle.TitleType.NodeTitleAttribute => NodeTitleAttribute.GetTitle(nodeComponent.Node),
                        NodeTitle.TitleType.CustomTitle => titleComponent.CustomTitle,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                });
            }
        }
    }
}

#endif