using UnityEngine;

namespace GraphExt.Prefab
{
    public class NodePosition : MonoBehaviour, INodeProperty
    {
        public Vector2 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
    }
}