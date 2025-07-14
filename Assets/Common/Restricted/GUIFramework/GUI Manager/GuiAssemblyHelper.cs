using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevCommon.Extended;
using UnityEditor;
using UnityEngine;
using DevCommon.Utils;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DevCommon.GUI
{
    public class GuiAssemblyHelper
    {
#if UNITY_EDITOR
        // Refresh gui assembly
        public static void RefreshGuiAssembly(CanvasController a_Controller)
        {
            if (a_Controller.ResourcesDirectory == null || a_Controller.AssemblyDirectory == null)
            {
                Debug.LogError("<color=#ff1500>Resources or Assembly Directory can't be Null!</color>");
                return;
            }

            tempLoadAllCanvas(a_Controller, (result) =>
            {
                bool t_OnCompileCanvasScreens = false;
                bool t_OnCompileSelectables = false;

                List<EnumAttribute> t_EnumAttributes = new List<EnumAttribute>();
                List<EnumAttribute> t_CanvasScreenEnumAttributes = getCanvasScreenEnumAttributes(a_Controller, a_Controller.ControllerName, out t_OnCompileCanvasScreens);
                List<EnumAttribute> t_CanvasScreenSelectablesEnumAttributes = getCanvasScreenSelectablesEnumAttributes(a_Controller, out t_OnCompileSelectables);

                if (t_CanvasScreenEnumAttributes.Any())
                    t_EnumAttributes.AddRange(t_CanvasScreenEnumAttributes);
                if (t_CanvasScreenEnumAttributes.Any())
                    t_EnumAttributes.AddRange(t_CanvasScreenSelectablesEnumAttributes);

                if (!t_OnCompileCanvasScreens || !t_OnCompileSelectables)
                {
                    Debug.LogError("<color=#ff1500>GUI Assembly Compilation Failed!</color>");
                    return;
                }
                if (t_EnumAttributes.Any())
                {
                    unloadAllTempCanvas(result);
                    CEnumBuilder.CreateEnum(AssetDatabase.GetAssetPath(a_Controller.AssemblyDirectory), $"DevCommon.GUI.{a_Controller.ControllerName}", t_EnumAttributes);
                    AssetDatabase.Refresh();
                }
            });
        }

        // Get canvas screens enum attributes
        private static List<EnumAttribute> getCanvasScreenEnumAttributes(MonoBehaviour a_Behaviour, string a_ControllerName, out bool a_ReCreateAssembly)
        {
            List<CanvasBase> t_CanvasObjects = a_Behaviour.GetComponentsInChildren<CanvasBase>().ToList();
            List<EnumAttribute> t_EnumAttributes = new List<EnumAttribute>();

            if (t_CanvasObjects.Count != t_CanvasObjects.DistinctBy(x => x.name).Count())
            {
                Debug.LogError("More than one <color=#ff1500>Canvas</color> has same name!");
                a_ReCreateAssembly = false;
                return new List<EnumAttribute>();
            }

            EnumAttribute t_EnumAttribute = new EnumAttribute();
            t_EnumAttribute.Name = $"{a_ControllerName}.ECanvasScreen";

            for (int i = 0; i < t_CanvasObjects.Count; i++)
            {
                t_CanvasObjects.ElementAt(i).gameObject.name = t_CanvasObjects.ElementAt(i).gameObject.name.LetterOnly();
                t_EnumAttribute.EnumItems.Add(i, t_CanvasObjects.ElementAt(i).gameObject.name);
                if (Enum.Equals(PrefabUtility.GetPrefabAssetType(t_CanvasObjects.ElementAt(i).gameObject), PrefabAssetType.Regular))
                {
                    PrefabUtility.ApplyPrefabInstance(t_CanvasObjects.ElementAt(i).gameObject, InteractionMode.AutomatedAction);
                }
            }

            if (t_EnumAttribute.EnumItems.Any())
            {
                t_EnumAttributes.Add(t_EnumAttribute);
            }

            a_ReCreateAssembly = true;
            return t_EnumAttributes;
        }

        // Get canvas screen's selectables enum attributes
        private static List<EnumAttribute> getCanvasScreenSelectablesEnumAttributes(MonoBehaviour a_Behaviour, out bool a_ReCreateAssembly)
        {
            List<CanvasBase> t_CanvasObjects = a_Behaviour.GetComponentsInChildren<CanvasBase>().ToList();
            List<EnumAttribute> t_EnumAttributes = new List<EnumAttribute>();
            for (int i = 0; i < t_CanvasObjects.Count; i++)
            {
                EnumAttribute t_EnumAttribute = new EnumAttribute();
                List<Selectable> t_Selectables = getSelectables(t_CanvasObjects.ElementAt(i).gameObject);
                t_EnumAttribute.Name = "E" + t_CanvasObjects.ElementAt(i).gameObject.name;

                if (t_Selectables.Count != t_Selectables.DistinctBy(x => x.name).Count())
                {
                    Debug.LogError($"More than one <color=#ff1500>Selectable</color> has same name in <color=#0000ff>{t_CanvasObjects.ElementAt(i).name}</color> Canvas!");
                    a_ReCreateAssembly = false;
                    return new List<EnumAttribute>();
                }

                for (int j = 0; j < t_Selectables.Count; j++)
                {
                    t_Selectables.ElementAt(j).name = t_Selectables.ElementAt(j).name.LetterOnly();
                    t_EnumAttribute.EnumItems.Add(j, t_Selectables.ElementAt(j).name);
                }

                if (t_EnumAttribute.EnumItems.Any())
                {
                    t_EnumAttributes.Add(t_EnumAttribute);
                }
                if (Enum.Equals(PrefabUtility.GetPrefabAssetType(t_CanvasObjects.ElementAt(i).gameObject), PrefabAssetType.Regular))
                {
                    PrefabUtility.ApplyPrefabInstance(t_CanvasObjects.ElementAt(i).gameObject, InteractionMode.AutomatedAction);
                }
            }
            a_ReCreateAssembly = true;
            return t_EnumAttributes;
        }

        // Get selectables, which are not ignored by framework
        private static List<Selectable> getSelectables(GameObject a_GameObject)
        {
            List<Selectable> t_Selectables = a_GameObject.GetComponentsInChildren<Selectable>().ToList();
            t_Selectables.RemoveAll(x => x.GetComponent<GuiElementProperty>() != null && x.GetComponent<GuiElementProperty>().Ignored);
            return t_Selectables;
        }

        // Temporarily load all canvas
        private static void tempLoadAllCanvas(CanvasController a_Controller, Action<List<GuiCanvasData>> a_OnComplete)
        {
            List<CanvasBase> t_HierarchyCanvasObjs = a_Controller.GetComponentsInChildren<CanvasBase>().ToList();
            List<GuiCanvasData> t_GuiCanvasDatas = new List<GuiCanvasData>();
            t_HierarchyCanvasObjs.ForEach(x => t_GuiCanvasDatas.Add(new GuiCanvasData(x, null)));
            List<FileInfo> t_FilesInfo = new List<FileInfo>();

            fetchAndFilterResCanvasObjs(a_Controller, t_HierarchyCanvasObjs, out t_FilesInfo);
            instantiateResCanvasObjs(a_Controller, t_FilesInfo, ref t_GuiCanvasDatas);

            a_OnComplete?.Invoke(t_GuiCanvasDatas);
        }

        // Fetch and filter Resources Canvas Objs against Hierarchy Canvas Objs
        private static void fetchAndFilterResCanvasObjs(CanvasController a_Controller, List<CanvasBase> a_HierarchyCanvasObjs, out List<FileInfo> a_FilesInfo)
        {
            string t_AssetDirPath = AssetDatabase.GetAssetPath(a_Controller.ResourcesDirectory);
            DirectoryInfo t_DirInfo = new DirectoryInfo(t_AssetDirPath);
            a_FilesInfo = t_DirInfo.GetFiles("*.prefab").ToList();

            for (int i = 0; i < a_HierarchyCanvasObjs.Count; i++)
            {
                for (int j = 0; j < a_FilesInfo.Count; j++)
                {
                    if (a_FilesInfo[j].Name.Contains(a_HierarchyCanvasObjs[i].gameObject.name))
                    {
                        a_FilesInfo.Remove(a_FilesInfo[j]);
                        break;
                    }
                }
            }
        }

        // Instantiate Resources Canvas Objs
        private static void instantiateResCanvasObjs(CanvasController a_Controller, List<FileInfo> a_FilesInfo, ref List<GuiCanvasData> a_GuiCanvasDatas)
        {
            foreach (FileInfo item in a_FilesInfo)
            {
                string t_ResPath = AssetFetch.GetAssetPath(item.FullName);
                UnityEngine.Object t_ResObject = AssetDatabase.LoadAssetAtPath(t_ResPath, typeof(UnityEngine.Object));
                UnityEngine.Object t_PrefabObj = PrefabUtility.InstantiatePrefab(t_ResObject, a_Controller.ParentCanvas.transform);
                a_GuiCanvasDatas.Add(new GuiCanvasData(null, t_PrefabObj));
            }
        }

        // Unload all temporarily loaded canvas
        private static void unloadAllTempCanvas(List<GuiCanvasData> a_GuiCanvasDatas)
        {
            a_GuiCanvasDatas.ForEach(x =>
            {
                if (x.ResourceCanvasObj != null)
                {
                    UnityEngine.Object.DestroyImmediate(x.ResourceCanvasObj);
                }
            });
            a_GuiCanvasDatas.Clear();
        }
#endif
    }

    [System.Serializable]
    public class GuiCanvasData
    {
        public CanvasBase HierarchyCanvasObj;
        public UnityEngine.Object ResourceCanvasObj;

        public GuiCanvasData() {}

        public GuiCanvasData(CanvasBase a_HierarchyCanvasObj, UnityEngine.Object a_ResourceCanvasObj)
        {
            HierarchyCanvasObj = a_HierarchyCanvasObj;
            ResourceCanvasObj = a_ResourceCanvasObj;
        }
    }
}
