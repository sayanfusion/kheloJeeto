using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class ChangeSourceImageForImmediateChild : EditorWindow
{
    
    private Sprite selectedSprite;
    public GameObject[] gameObjectList;
    private int childIndex = 0;
    [MenuItem("Tools/Change Source Image for Immediate Child")]
    public static void ShowWindow()
    {
        GetWindow<ChangeSourceImageForImmediateChild>("Change Source Image for Immediate Child");
    }
    private void OnGUI()
    {
        GUILayout.Label("Change Source Image for Immediate Child", EditorStyles.boldLabel);
        // Field to select the sprite
        selectedSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", selectedSprite, typeof(Sprite), false);
        // List of GameObjects
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty gameObjectListProperty = serializedObject.FindProperty("gameObjectList");
        EditorGUILayout.PropertyField(gameObjectListProperty, new GUIContent("Game Objects"), true);
        serializedObject.ApplyModifiedProperties();
        // Child index
        childIndex = EditorGUILayout.IntField("Child Index", childIndex);
        // Apply button
        if (GUILayout.Button("Apply Image Settings"))
        {
            ApplyImageSettings();
        }
    }
    private void ApplyImageSettings()
    {
        if (selectedSprite == null)
        {
            Debug.LogError("Please select a sprite.");
            return;
        }
        if (gameObjectList == null || gameObjectList.Length == 0)
        {
            Debug.LogError("Please provide a list of GameObjects.");
            return;
        }
        foreach (var parentGameObject in gameObjectList)
        {
            if (parentGameObject == null) continue;
            // Check if the child index is valid
            if (parentGameObject.transform.childCount > childIndex)
            {
                Transform child = parentGameObject.transform.GetChild(childIndex);
                Image imageComponent = child.GetComponent<Image>();
                if (imageComponent != null)
                {
                    Undo.RecordObject(imageComponent, "Change Source Image");
                    // Apply the selected sprite
                    imageComponent.sprite = selectedSprite;
                    EditorUtility.SetDirty(imageComponent);
                }
                else
                {
                    Debug.LogWarning($"No Image component found on GameObject: {child.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Child index {childIndex} is out of bounds for GameObject: {parentGameObject.name}");
            }
        }
        Debug.Log("Image settings applied.");
    }
}