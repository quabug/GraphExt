using System.Collections.Generic;
using OneShot;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryGraphInstaller : IGraphInstaller
    {
        public void Install(Container container)
        {
            container.RegisterDictionaryInstance(new Dictionary<NodeId, Vector2>());
        }
    }
}