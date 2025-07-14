using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevCommon.Extended;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevCommon.Utils
{
    public class SceneHandler
    {
        private enum EProcessingMethod
        {
            Sync = 0,
            Async = 1,
        }

        private static Action<string> onLoadingComplete;
        private static Action<string> onUnloadingComplete;

        // Load scene
        public static void LoadScene(SceneAttribute a_Properties, LoadSceneMode a_LoadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.sceneLoaded += onSceneLoaded;
            onLoadingComplete = a_Properties.OnTaskComplete;
            SceneManager.LoadScene(a_Properties.SceneName, a_LoadSceneMode);
        }

        // Load scene async
        public static void LoadSceneAsync(ref SceneAsyncAttribute a_Properties, bool a_AllowSceneActivation, LoadSceneMode a_LoadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.sceneLoaded += onSceneLoaded;
            onLoadingComplete = a_Properties.OnTaskComplete;
            a_Properties.Behaviour.StartCoroutine(loadSceneAsynchonously(a_Properties, a_AllowSceneActivation, a_LoadSceneMode));
        }

        private static IEnumerator loadSceneAsynchonously(SceneAsyncAttribute a_Properties, bool a_AllowSceneActivation, LoadSceneMode a_LoadSceneMode)
        {
            AsyncOperation t_AsyncLoad = SceneManager.LoadSceneAsync(a_Properties.SceneName, a_LoadSceneMode);
            t_AsyncLoad.allowSceneActivation = a_AllowSceneActivation;

            while (!t_AsyncLoad.isDone)
            {
                a_Properties.OnTaskProgress?.Invoke(t_AsyncLoad.progress);
                t_AsyncLoad.allowSceneActivation = a_AllowSceneActivation;
                yield return null;
            }
        }

        private static void onSceneLoaded(Scene a_Scene, LoadSceneMode a_Mode)
        {
            onLoadingComplete?.Invoke(a_Scene.name);
            SceneManager.sceneLoaded -= onSceneLoaded;
        }

        // Unload scene async
        public static void UnloadSceneAsync(SceneAsyncAttribute a_Properties)
        {
            SceneManager.sceneUnloaded += onSceneUnloaded;
            onUnloadingComplete = a_Properties.OnTaskComplete;
            a_Properties.Behaviour.StartCoroutine(unloadSceneAsynchonously(a_Properties));
        }

        private static IEnumerator unloadSceneAsynchonously(SceneAsyncAttribute a_Properties)
        {
            AsyncOperation t_AsyncLoad = SceneManager.UnloadSceneAsync(a_Properties.SceneName);
            while (!t_AsyncLoad.isDone)
            {
                a_Properties.OnTaskProgress?.Invoke(t_AsyncLoad.progress);
                yield return null;
            }
        }

        private static void onSceneUnloaded(Scene a_Scene)
        {
            onUnloadingComplete?.Invoke(a_Scene.name);
            SceneManager.sceneUnloaded -= onSceneUnloaded;
        }

#if UNITY_EDITOR
        [MenuItem("DevCommon/Refresh Scene Assembly")]
        private static void refreshSceneAssembly()
        {
            int t_Count = SceneManager.sceneCountInBuildSettings;
            List<string> t_ScenesName = new List<string>();
            for (int i = 0; i < t_Count; i++)
                t_ScenesName.Add(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));

            if (t_ScenesName.Any())
            {
                EnumAttribute t_EnumAttribute = new EnumAttribute();
                t_EnumAttribute.Name = $"EScene";

                for (int i = 0; i < t_ScenesName.Count; i++)
                    t_EnumAttribute.EnumItems.Add(i, t_ScenesName[i]);

                CEnumBuilder.CreateEnum(System.IO.Path.GetFileName(Application.dataPath), "DevCommon.Scene.Assembly", t_EnumAttribute);
                AssetDatabase.Refresh();
            }
        }
#endif
    }

    public class SceneAttribute
    {
        public string SceneName;
        public Action<string> OnTaskComplete;
    }

    public class SceneAsyncAttribute : SceneAttribute
    {
        public Action<float> OnTaskProgress;
        public MonoBehaviour Behaviour;
    }
}