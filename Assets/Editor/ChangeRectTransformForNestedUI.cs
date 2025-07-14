using UnityEngine;
using UnityEditor;

public class ChangeRectTransformForNestedUI : EditorWindow
{
    public GameObject[] gameObjectList;

    private bool isNestedChild = false;
    private int firstChildIndex = 0;
    private int secondChildIndex = 0;

    private float width = 100f;
    private float height = 100f;
    private Vector3 scale = Vector3.one;
    private float posX = 0f;
    private float posY = 0f;

    private AnchorPreset anchorPreset = AnchorPreset.MiddleCenter;

    [MenuItem("Tools/Change RectTransform for Nested UI")]
    public static void ShowWindow()
    {
        GetWindow<ChangeRectTransformForNestedUI>("Change RectTransform for Nested UI");
    }

    private void OnGUI()
    {
        GUILayout.Label("Change RectTransform for Nested UI", EditorStyles.boldLabel);

        // RectTransform size fields
        width = EditorGUILayout.FloatField("Width", width);
        height = EditorGUILayout.FloatField("Height", height);

        // Scale fields
        scale = EditorGUILayout.Vector3Field("Scale", scale);

        // Position fields
        posX = EditorGUILayout.FloatField("Position X", posX);
        posY = EditorGUILayout.FloatField("Position Y", posY);

        // Anchor position dropdown
        anchorPreset = (AnchorPreset)EditorGUILayout.EnumPopup("Anchor Preset", anchorPreset);

        // GameObject array input
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty gameObjectListProperty = serializedObject.FindProperty("gameObjectList");
        EditorGUILayout.PropertyField(gameObjectListProperty, new GUIContent("Game Objects"), true);
        serializedObject.ApplyModifiedProperties();

        // Toggle for nested child
        isNestedChild = EditorGUILayout.Toggle("Is Nested Child", isNestedChild);

        // Child indices
        firstChildIndex = EditorGUILayout.IntField("First Child Index", firstChildIndex);
        if (isNestedChild)
        {
            secondChildIndex = EditorGUILayout.IntField("Second Child Index", secondChildIndex);
        }

        // Apply button
        if (GUILayout.Button("Apply RectTransform Settings"))
        {
            ApplyRectTransformSettings();
        }
    }

    private void ApplyRectTransformSettings()
    {
        if (gameObjectList == null || gameObjectList.Length == 0)
        {
            Debug.LogError("Please provide a list of GameObjects.");
            return;
        }

        foreach (var parentGameObject in gameObjectList)
        {
            if (parentGameObject == null) continue;

            if (isNestedChild)
            {
                // Get the first child
                if (parentGameObject.transform.childCount > firstChildIndex)
                {
                    Transform firstChild = parentGameObject.transform.GetChild(firstChildIndex);

                    // Get the second child
                    if (firstChild.childCount > secondChildIndex)
                    {
                        Transform secondChild = firstChild.GetChild(secondChildIndex);
                        ApplyTransformSettings(secondChild);
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
            else
            {
                // Get the immediate child
                if (parentGameObject.transform.childCount > firstChildIndex)
                {
                    Transform immediateChild = parentGameObject.transform.GetChild(firstChildIndex);
                    ApplyTransformSettings(immediateChild);
                }
                else
                {
                    Debug.LogWarning($"Child index {firstChildIndex} is out of bounds for GameObject: {parentGameObject.name}");
                }
            }
        }

        Debug.Log("RectTransform settings applied.");
    }

    private void ApplyTransformSettings(Transform targetTransform)
    {
        RectTransform rectTransform = targetTransform.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            Undo.RecordObject(rectTransform, "Change RectTransform Properties");

            // Apply width and height
            rectTransform.sizeDelta = new Vector2(width, height);

            // Apply scale
            rectTransform.localScale = scale;

            // Apply position
            rectTransform.anchoredPosition = new Vector2(posX, posY);

            // Apply anchor preset
            SetAnchorPreset(rectTransform, anchorPreset);

            EditorUtility.SetDirty(rectTransform);
        }
        else
        {
            Debug.LogWarning($"No RectTransform component found on GameObject: {targetTransform.name}");
        }
    }

    private void SetAnchorPreset(RectTransform rectTransform, AnchorPreset preset)
    {
        switch (preset)
        {
            case AnchorPreset.TopLeft:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                break;
            case AnchorPreset.TopCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                break;
            case AnchorPreset.TopRight:
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case AnchorPreset.MiddleLeft:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                break;
            case AnchorPreset.MiddleCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                break;
            case AnchorPreset.MiddleRight:
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                break;
            case AnchorPreset.BottomLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                break;
            case AnchorPreset.BottomCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                break;
            case AnchorPreset.BottomRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                break;
            case AnchorPreset.StretchTop:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case AnchorPreset.StretchMiddle:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                break;
            case AnchorPreset.StretchBottom:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                break;
            case AnchorPreset.StretchFull:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
        }
    }

    private enum AnchorPreset
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
        StretchTop,
        StretchMiddle,
        StretchBottom,
        StretchFull
    }
}
