using UnityEngine;
using UnityEngine.UI; 
using UnityEditor;

public class ChangeFontForNestedUIText : EditorWindow
{
    
    private Font selectedFont;

    
    public GameObject[] gameObjectList;

    
    private int firstChildIndex = 0;
    private int secondChildIndex = 0;

    
    private int fontSize = 14; 
    private int maxFontSize = 100; 
    private bool enableBestFit = false; 

    [MenuItem("Tools/Change Font for Nested UI Text")]
    public static void ShowWindow()
    {
        GetWindow<ChangeFontForNestedUIText>("Change Font for Nested UI Text");
    }

    private void OnGUI()
    {
        GUILayout.Label("Change Font for Nested UI Text", EditorStyles.boldLabel);

        
        selectedFont = (Font)EditorGUILayout.ObjectField("Font", selectedFont, typeof(Font), false);

        
        fontSize = EditorGUILayout.IntField("Font Size", fontSize);
        enableBestFit = EditorGUILayout.Toggle("Enable Best Fit", enableBestFit);
        if (enableBestFit)
        {
            maxFontSize = EditorGUILayout.IntField("Max Font Size", maxFontSize);
        }

        
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty gameObjectListProperty = serializedObject.FindProperty("gameObjectList");
        EditorGUILayout.PropertyField(gameObjectListProperty, new GUIContent("Game Objects"), true);
        serializedObject.ApplyModifiedProperties();

        
        firstChildIndex = EditorGUILayout.IntField("First Child Index", firstChildIndex);
        secondChildIndex = EditorGUILayout.IntField("Second Child Index", secondChildIndex);

        
        if (GUILayout.Button("Apply Font Settings"))
        {
            ApplyFontSettings();
        }
    }

    private void ApplyFontSettings()
    {
        if (selectedFont == null)
        {
            Debug.LogError("Please select a font.");
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

            
            if (parentGameObject.transform.childCount > firstChildIndex)
            {
                Transform firstChild = parentGameObject.transform.GetChild(firstChildIndex);

                
                if (firstChild.childCount > secondChildIndex)
                {
                    Transform secondChild = firstChild.GetChild(secondChildIndex);
                    Text textComponent = secondChild.GetComponent<Text>();

                    if (textComponent != null)
                    {
                        Undo.RecordObject(textComponent, "Change Font and Size");

                        
                        textComponent.font = selectedFont;

                        
                        textComponent.fontSize = fontSize;

                        
                        textComponent.resizeTextForBestFit = enableBestFit;
                        if (enableBestFit)
                        {
                            textComponent.resizeTextMaxSize = maxFontSize;
                        }

                        EditorUtility.SetDirty(textComponent);
                    }
                    else
                    {
                        Debug.LogWarning($"No Text component found on GameObject: {secondChild.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Second child index {secondChildIndex} is out of bounds for GameObject: {firstChild.name}");
                }
            }
            else
            {
                Debug.LogWarning($"First child index {firstChildIndex} is out of bounds for GameObject: {parentGameObject.name}");
            }
        }

        Debug.Log("Font settings applied.");
    }
}
