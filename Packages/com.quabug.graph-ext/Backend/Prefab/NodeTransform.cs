using System;
using System.Linq;
using UnityEngine;

namespace GraphExt
{
    public static class NodeTransform
    {
        public static Action<Transform> ReorderChildrenTransformAction<TKey>(Func<INodeComponent, TKey> order)
        {
            var isTransforming = false;
            return parent =>
            {
                if (isTransforming) return;
                isTransforming = true;
                foreach (var child in parent.GetComponentsInChildren<INodeComponent>().OrderBy(order))
                {
                    ((Component)child).transform.SetAsLastSibling();
                }
                isTransforming = false;
            };
        }
    }
}