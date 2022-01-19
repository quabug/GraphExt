using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    public static class Window
    {
        public static T GetOrCreate<T>(string title, params Type[] desiredDockNextTo) where T : EditorWindow
        {
            var window = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(w => w.titleContent.text == title);
            if (window == null) window = EditorWindow.CreateWindow<T>(title, desiredDockNextTo);
            return window;
        }

        public static EditorWindow GetOrCreate([NotNull] Type windowType, string title, params Type[] desiredDockNextTo)
        {
            var window = Resources.FindObjectsOfTypeAll(windowType).OfType<EditorWindow>().FirstOrDefault(w => w.titleContent.text == title);
            if (window == null) window = CreateWindow(windowType, title, desiredDockNextTo);
            return window;
        }

        public static EditorWindow CreateWindow(Type windowType, string title, params Type[] desiredDockNextTo)
        {
            var method = typeof(EditorWindow).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(mi => mi.Name == nameof(EditorWindow.CreateWindow) && mi.IsGenericMethod && mi.GetParameters().Length == 2)
                .MakeGenericMethod(windowType)
            ;
            return (EditorWindow) method.Invoke(null, new object[] { title, desiredDockNextTo });
        }
    }
}