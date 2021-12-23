using System;
using UnityEngine;

namespace GraphExt.Prefab
{
    public interface IGameObjectNode {}

    public class GameObjectNode : MonoBehaviour
    {
        [SerializeReference] public IGameObjectNode Node;
        [SerializeField, HideInInspector] private string _nodeId = Guid.NewGuid().ToString();
        public NodeId Id => Guid.Parse(_nodeId);
    }
}