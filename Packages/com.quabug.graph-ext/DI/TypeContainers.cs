using System;
using System.Collections.Generic;
using OneShot;

namespace GraphExt.Editor
{
    public class TypeContainers
    {
        private readonly Dictionary<Type, Container> _typeContainers = new Dictionary<Type, Container>();
        public IReadOnlyDictionary<Type, Container> Containers => _typeContainers;
        public Container this[Type type] => Containers[type];

        public Container CreateTypeContainer(Container parent, params Type[] types)
        {
            var child = parent.CreateChildContainer();
            foreach (var type in types) _typeContainers.Add(type, child);
            return child;
        }

        public Container GetTypeContainer(Type type)
        {
            return _typeContainers[type];
        }

        public Container CreateSystemContainer(Container parent, params Type[] systemTypes)
        {
            var systemContainer = parent.CreateChildContainer();
            foreach (var type in systemTypes)
            {
                if (!typeof(IWindowSystem).IsAssignableFrom(type)) throw new ArgumentException();
                _typeContainers.Add(type, systemContainer);
                systemContainer.RegisterSingleton(type);
                parent.Register(() => (IWindowSystem) systemContainer.Resolve(type));
            }
            return systemContainer;
        }
    }
}