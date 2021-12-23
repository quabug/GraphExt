using System.Collections.Generic;
using GraphExt.Memory;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class GameObjectNode : MonoBehaviour
    {
        [SerializeReference] public IMemoryNode Node;
        public NodeId Id => Node.Id;
        public IEnumerable<INodeProperty> Properties { get; }
    }
}