using UnityEditor;
using UnityEngine;

namespace DevCommon.GUI
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(CanvasController), true)]
    public class CanvasControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CanvasController t_CanvasController = (CanvasController)target;

            var t_ButtonStyle = new GUIStyle(UnityEngine.GUI.skin.button);
            t_ButtonStyle.normal.textColor = Color.black;
            UnityEngine.GUI.backgroundColor = Color.green;

            GUILayout.FlexibleSpace();
#if UNITY_EDITOR
            if (GUILayout.Button("Refresh GUI Assembly", t_ButtonStyle))
            {
                t_CanvasController.RefreshGuiAssembly();
            }
#endif
        }
    }
}
