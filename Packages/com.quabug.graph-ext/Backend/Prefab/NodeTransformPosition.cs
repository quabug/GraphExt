using System.Collections.Generic;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class NodeTransformPosition : MonoBehaviour, INodePropertyContainer
    {
        public IEnumerable<INodeProperty> Properties =>
            new NodePositionProperty(() => transform.position, pos => transform.position = pos).Yield();
    }
}