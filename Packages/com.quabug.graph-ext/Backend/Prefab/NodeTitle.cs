using UnityEngine;

namespace GraphExt
{
    public class NodeTitle : MonoBehaviour
    {
        public enum TitleType { Hidden, GameObjectName, NodeTitleAttribute, CustomTitle }
        public TitleType Type;
        public string CustomTitle;
    }
}