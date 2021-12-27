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

        public delegate void OnNodeSelectedChangedFunc(in NodeId nodeId, bool isSelected);
        public OnNodeSelectedChangedFunc OnNodeSelectedChanged;

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
        }

        public void Dispose()
        {
            GameObjectNodes.Dispose();
        }

        public void AddGameObjectNode(in NodeId nodeId, TNode node, Vector2 position)
        {
            var ports = FindNodePorts(node).ToArray();
            foreach (var port in ports) _PortData[new PortId(nodeId, port.Name)] = port;
            Runtime.AddNode(nodeId, node);
            GameObjectNodes[nodeId].Position = position;
            _NodeData[nodeId] = ToNodeData(nodeId, node);
            Utility.SavePrefabStage();
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
            return NodePortUtility.FindPorts(node.GetType());
        }

        protected override NodeData ToNodeData(in NodeId id, TNode node)
        {
            var nodeObject = GameObjectNodes[id];
            var nodeComponent = nodeObject.GetComponent<TComponent>();
            var nodeSerializedProperty = new SerializedObject(nodeComponent).FindProperty(nodeComponent.NodeSerializedPropertyName);
            var nodeId = id;
            return new NodeData(CreateNodeSelector().Yield()
                .Append<INodeProperty>(new NodePositionProperty(nodeComponent.Position.x, nodeComponent.Position.y))
                .Append(CreateTitleProperty())
                .Concat(NodePropertyUtility.CreateProperties(node, id, nodeSerializedProperty))
                .ToArray()
            );

            NodeSelector CreateNodeSelector()
            {
                var selector = new NodeSelector();
                selector.OnSelectChanged += isSelected => OnNodeSelectedChanged?.Invoke(nodeId, isSelected);
                return selector;
            }

            DynamicTitleProperty CreateTitleProperty()
            {
                return new DynamicTitleProperty(() =>
                {
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