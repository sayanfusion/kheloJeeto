using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectAttachedImagesTool : EditorWindow
{
    [MenuItem("Tools/Select Attached Images")]
    public static void ShowWindow()
    {
        GetWindow<SelectAttachedImagesTool>("Select Attached Images");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Select Attached Images Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Select Images from Selected GameObjects"))
        {
            SelectImagesFromSelectedGameObjects();
        }

        EditorGUILayout.HelpBox("This tool selects all the images from SpriteRenderer or Image components attached to the selected GameObjects and their children in the hierarchy.", MessageType.Info);
    }

    private static void SelectImagesFromSelectedGameObjects()
    {
        GameObject[] selectedGameObjects = Selection.gameObjects;
        List<Object> imagesToSelect = new List<Object>();

        foreach (GameObject gameObject in selectedGameObjects)
        {
            FindImagesInGameObjectAndChildren(gameObject, imagesToSelect);
        }

        // Filter to include only image assets
        imagesToSelect = FilterImageAssets(imagesToSelect);

        if (imagesToSelect.Count > 0)
        {
            Selection.objects = imagesToSelect.ToArray();
            Debug.Log($"Selected {imagesToSelect.Count} image(s) in the Project window.");
        }
        else
        {
            Debug.LogWarning("No images found in the selected GameObjects or their children.");
        }
    }

    private static void FindImagesInGameObjectAndChildren(GameObject gameObject, List<Object> imagesToSelect)
    {
        // Check for SpriteRenderer
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            string spritePath = AssetDatabase.GetAssetPath(spriteRenderer.sprite);
            Object spriteAsset = AssetDatabase.LoadAssetAtPath<Object>(spritePath);
            if (spriteAsset != null)
            {
                imagesToSelect.Add(spriteAsset);
            }
        }

        // Check for Image (UI)
        Image uiImage = gameObject.GetComponent<Image>();
        if (uiImage != null && uiImage.sprite != null)
        {
            string spritePath = AssetDatabase.GetAssetPath(uiImage.sprite);
            Object spriteAsset = AssetDatabase.LoadAssetAtPath<Object>(spritePath);
            if (spriteAsset != null)
            {
                imagesToSelect.Add(spriteAsset);
            }
        }

        // Recursively check children
        foreach (Transform child in gameObject.transform)
        {
            FindImagesInGameObjectAndChildren(child.gameObject, imagesToSelect);
        }
    }

    private static List<Object> FilterImageAssets(List<Object> objects)
    {
        List<Object> imageAssets = new List<Object>();

        foreach (Object obj in objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".tga") || path.EndsWith(".gif"))
            {
                imageAssets.Add(obj);
            }
        }

        return imageAssets;
    }
}
