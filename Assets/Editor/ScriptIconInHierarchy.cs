using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
public class ScriptIconInHierarchy
{
    private static readonly Texture2D ScriptIcon;
    private static readonly Dictionary<GameObject, MonoBehaviour[]> CachedScripts = new Dictionary<GameObject, MonoBehaviour[]>();

    static ScriptIconInHierarchy()
    {
        ScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

        // Subscribe to events
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        EditorApplication.hierarchyChanged += ClearCache;
    }

    private static void ClearCache()
    {
        CachedScripts.Clear();
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        // Skip objects with no MonoBehaviours
        if (!obj.TryGetComponent<MonoBehaviour>(out _)) return;

        MonoBehaviour[] customScripts = GetCustomMonoBehaviours(obj);
        if (customScripts.Length > 0)
        {
            Rect iconRect = new Rect(selectionRect.xMax - 20, selectionRect.y, 16, 16);
            GUI.DrawTexture(iconRect, ScriptIcon);

            string tooltip = string.Join("\n", GetScriptNames(customScripts));
            EditorGUI.LabelField(iconRect, new GUIContent("", tooltip));
        }
    }
    private static MonoBehaviour[] GetCustomMonoBehaviours(GameObject obj)
    {
        if (!CachedScripts.TryGetValue(obj, out MonoBehaviour[] scripts))
        {
            scripts = System.Array.FindAll(obj.GetComponents<MonoBehaviour>(), script =>
                script != null && IsCustomScript(script)
            );
            CachedScripts[obj] = scripts;
        }
        return scripts;
    }

    private static bool IsCustomScript(MonoBehaviour script)
    {
        string assemblyName = script.GetType().Assembly.GetName().Name;
        return assemblyName == "Assembly-CSharp" || assemblyName.StartsWith("Assembly-CSharp-");
    }

    private static string[] GetScriptNames(MonoBehaviour[] scripts)
    {
        var scriptNames = new List<string>();
        foreach (var script in scripts)
        {
            if (script != null)
            {
                scriptNames.Add(script.GetType().Name);
            }
        }
        return scriptNames.ToArray();
    }
}