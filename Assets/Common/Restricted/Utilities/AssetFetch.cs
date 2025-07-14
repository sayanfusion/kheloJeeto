using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DevCommon.Utils
{
    public class AssetFetch
    {
#if UNITY_EDITOR
        // Get proper asset folder path
        public static string GetAssetPath(string a_FullPath, string a_StartRegion = "Assets")
        {
            var t_Parts = a_FullPath.Split(Path.DirectorySeparatorChar);
            int t_AfterIndex = Array.IndexOf(t_Parts, a_StartRegion);

            if (t_AfterIndex == -1)
            {
                return null;
            }

            return string.Join(Path.DirectorySeparatorChar.ToString(),
            t_Parts, t_AfterIndex, t_Parts.Length - t_AfterIndex);
        }

        /// <summary>
        /// Adds newly (if not already in the list) found assets.
        /// Returns how many found (not how many added)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a_Path"></param>
        /// <param name="a_Assets">Adds to this list if it is not already there</param>
        /// <returns></returns>
        public static int TryGetUnityObjectsOfTypeFromPath<T>(string a_Path, out List<T> a_Assets) where T : UnityEngine.Object
        {
            a_Assets = new List<T>();
            string[] t_FilePaths = System.IO.Directory.GetFiles(a_Path);
            int t_CountFound = 0;

            Debug.Log(t_FilePaths.Length);

            if (t_FilePaths != null && t_FilePaths.Length > 0)
            {
                for (int i = 0; i < t_FilePaths.Length; i++)
                {
                    UnityEngine.Object t_ResObj = UnityEditor.AssetDatabase.LoadAssetAtPath(t_FilePaths[i], typeof(T));
                    if (t_ResObj is T asset)
                    {
                        t_CountFound++;
                        if (!a_Assets.Contains(asset))
                        {
                            a_Assets.Add(asset);
                        }
                    }
                }
            }

            return t_CountFound;
        }

        /// <summary>
        /// Tries the get an asset.
        /// </summary>
        /// <returns>The asset.</returns>
        /// <param name="optionalName">Optional name: if no name provided returns the first asset.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T TryGetAsset<T>(string optionalName = "") where T : UnityEngine.Object
        {
            // Gets all files with the Directory System.IO class
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
            T asset = null;

            // move through all files
            foreach (var file in files)
            {
                // use the GetRightPartOfPath utility method to cut the path so it looks like this: Assets/folderblah
                string path = GetAssetPath(file, "Assets");

                // Then I try and load the asset at the current path.
                asset = AssetDatabase.LoadAssetAtPath<T>(path);

                // check the asset to see if it's not null
                if (asset)
                {
                    // if the optional name is nothing then we skip this step
                    if (optionalName != "")
                    {
                        if (asset.name == optionalName)
                        {

                            Debug.Log("Found the database at path: " + path + "with name: " + asset.name);
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("Found the database at path: " + path);
                        break;
                    }
                }

            }

            return asset;
        }
#endif
    }
}