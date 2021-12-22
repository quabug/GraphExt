using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class GameObjectNode : MonoBehaviour, INodeData
    {
        [SerializeField, HideInInspector] private string _id = Guid.NewGuid().ToString();
        [field: SerializeField] public string UXMLPath { get; private set; }
        public NodeId Id => Guid.Parse(_id);
        public IReadOnlyList<INodeProperty> Properties { get; }
        public IReadOnlyList<PortData> Ports { get; }
    }
}