using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace GraphExt.Editor
{
    public interface IWindowExtension
    {
        void OnInitialized([NotNull] GraphWindow window, [NotNull] GraphConfig config, [NotNull] GraphView view);
        void OnClosed([NotNull] GraphWindow window, [NotNull] GraphConfig config, [NotNull] GraphView view);
    }

    [Serializable]
    public class GroupWindowExtension : IWindowExtension
    {
        [SerializeReference] internal List<IWindowExtension> Extensions = new List<IWindowExtension>();

        public T GetOrCreate<T>() where T : IWindowExtension
        {
            return (T) GetOrCreate(typeof(T));
        }

        public IWindowExtension AddIfNotExist(IWindowExtension extension)
        {
            var ext = Extensions.SingleOrDefault(ext => ext.GetType() == extension.GetType());
            if (ext == null) Extensions.Add(extension);
            return ext ?? extension;
        }

        internal IWindowExtension GetOrCreate(Type type)
        {
            Assert.IsTrue(typeof(IWindowExtension).IsAssignableFrom(type));
            var ext = Extensions.SingleOrDefault(ext => ext.GetType() == type);
            if (ext == null)
            {
                ext = (IWindowExtension) Activator.CreateInstance(type);
                Extensions.Add(ext);
            }
            return ext;
        }

        public void Remove(IWindowExtension extension)
        {
            Extensions.Remove(extension);
        }

        public void Remove<T>() where T : IWindowExtension
        {
            var index = Extensions.FindIndex(ext => ext is T);
            if (index >= 0) Extensions.RemoveAt(index);
        }

        public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
        {
            foreach (var ext in Extensions) ext.OnInitialized(window, config, view);
        }

        public void OnClosed(GraphWindow window, GraphConfig config, GraphView view)
        {
            foreach (var ext in Extensions) ext.OnClosed(window, config, view);
        }
    }
}