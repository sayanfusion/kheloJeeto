using System.Linq;
using UnityEngine;

/// <summary>
/// Abstract class for making reload-proof singletons out of ScriptableObjects
/// Returns the asset created on the editor, or null if there is none
/// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
/// </summary>
/// <typeparam name="T">Singleton type</typeparam>

namespace DevCommon
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        static bool isQuiting = false;
        static T instance = null;

        public static T Instance
        {
            get
            {
                if (isQuiting)
                {
                    return null;
                }

                if (!instance)
                    instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                return instance;
            }
        }

        protected void OnApplicationQuit()
        {
            isQuiting = true;
        }
    }
}