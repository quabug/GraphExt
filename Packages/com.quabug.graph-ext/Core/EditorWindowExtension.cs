using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GraphExt
{
    public static class Window
    {
        public static T GetOrCreate<T>(string title, params System.Type[] desiredDockNextTo) where T : EditorWindow
        {
            var window = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(w => w.titleContent.text == title);
            if (window == null) window = EditorWindow.CreateWindow<T>(title, desiredDockNextTo);
            return window;
        }
    }
}