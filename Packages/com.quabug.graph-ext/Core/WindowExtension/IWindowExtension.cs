using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt
{
    public interface IWindowExtension
    {
        void OnInitialized([NotNull] GraphWindow window, [NotNull] GraphConfig config, [NotNull] GraphView view);
    }

    [Serializable]
    public class GroupWindowExtension
    {
        [SerializeReference] public List<IWindowExtension> Extensions = new List<IWindowExtension>();

        public T GetOrCreate<T>() where T : IWindowExtension
        {
            var ext = (T) Extensions.SingleOrDefault(ext => ext is T);
            if (ext == null)
            {
                ext = Activator.CreateInstance<T>();
                Extensions.Add(ext);
            }
            return ext;
        }

        public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
        {
            foreach (var ext in Extensions) ext.OnInitialized(window, config, view);
        }
    }
}