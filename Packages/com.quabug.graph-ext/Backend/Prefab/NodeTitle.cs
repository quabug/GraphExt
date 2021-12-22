using System.Collections.Generic;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class NodeTitle : MonoBehaviour, INodePropertyContainer
    {
        public IEnumerable<INodeProperty> Properties => new TitleProperty(name).Yield();
    }
}