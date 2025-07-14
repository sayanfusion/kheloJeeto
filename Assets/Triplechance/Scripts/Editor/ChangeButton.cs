
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonImageChange))]
public class ChangeButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ButtonImageChange myScript = (ButtonImageChange)target;
        if (GUILayout.Button("Change Button"))
        {
            myScript.ChangeButton();
        }
    }

}
