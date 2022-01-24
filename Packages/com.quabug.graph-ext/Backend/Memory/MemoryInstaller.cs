using System.Collections.Generic;
using OneShot;
using UnityEngine;

namespace GraphExt.Editor
{
    public class MemoryInstaller : IInstaller
    {
        public void Install(Container container)
        {
            container.RegisterDictionaryInstance(new Dictionary<NodeId, Vector2>());
        }
    }
}