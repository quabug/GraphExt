#if UNITY_EDITOR

using UnityEngine.SceneManagement;

namespace GraphExt.Editor
{
    internal static class EditorUtility
    {
        public static void SavePrefabStage()
        {
            var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null) SaveScene(stage.scene);
        }

        public static void SaveScene(this Scene scene)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        }
    }
}

#endif