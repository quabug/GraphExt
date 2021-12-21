using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace GraphExt.Editor
{
    public static class Utilities
    {
        [Pure]
        public static string GetCurrentFilePath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
        {
            return fileName;
        }

        [Pure]
        public static string GetCurrentDirectoryPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
        {
            return Path.GetDirectoryName(fileName);
        }

        [Pure]
        public static string GetCurrentDirectoryProjectRelativePath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
        {
            return GetProjectRelativePath(Path.GetDirectoryName(fileName));
        }

        [Pure]
        public static string GetProjectRelativePath(string path)
        {
            var projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            return path.Substring(projectPath.Length);
        }
    }
}
