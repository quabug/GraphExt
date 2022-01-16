using UnityEngine;

namespace GraphExt
{
    public class NodeTitle : MonoBehaviour
    {
        public enum TitleType { Hidden, GameObjectName, CustomTitle }
        public TitleType Type;
        public string CustomTitle;
    }
}