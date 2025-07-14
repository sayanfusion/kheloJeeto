using UnityEngine;
using UnityEditor;

namespace DevCommon.Utils
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(DebugController), true)]
    public class DebugControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DebugController t_DebugController = (DebugController)target;

            var t_ButtonStyle = new GUIStyle(UnityEngine.GUI.skin.button);
            t_ButtonStyle.normal.textColor = Color.white;
            UnityEngine.GUI.backgroundColor = Color.red;

            GUILayout.FlexibleSpace();
#if UNITY_EDITOR
            if (GUILayout.Button("Clear Log Files", t_ButtonStyle))
                t_DebugController.ClearLogFiles();
#endif
        }
    }
}