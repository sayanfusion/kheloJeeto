using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class ChildCreator : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private Block prefab;
    [SerializeField] private Sprite pink;

    [ContextMenu("Create 1000 Children")]
    private void Create1000Children()
    {
#if UNITY_EDITOR
        if (!ValidateInput()) return;

        ClearChildren();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Block child = (Block)PrefabUtility.InstantiatePrefab(prefab, parent.gameObject.scene);
                child.transform.SetParent(parent);
                child.transform.localPosition = new Vector3(i * 100, -j * 100, 0); // optional layout spacing

                if ((i + j) % 2 != 0)
                    child.transform.GetChild(0).GetComponent<Image>().sprite = pink;

                string label = $"0{i}{j}";
                child.normalStateNum.text = label;
                child.name = label;

                Undo.RegisterCreatedObjectUndo(child.gameObject, "Create Block");
            }
        }

        EditorSceneManager.MarkSceneDirty(parent.gameObject.scene);
        Debug.Log("Created 100 children.");
#else
        Debug.LogWarning("Editor-only functionality.");
#endif
    }

    [ContextMenu("Create 100 Children")]
    private void Create100Children()
    {
#if UNITY_EDITOR
        if (!ValidateInput()) return;

        ClearChildren();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Block child = (Block)PrefabUtility.InstantiatePrefab(prefab, parent.gameObject.scene);
                child.transform.SetParent(parent);
                child.transform.localPosition = new Vector3(i * 100, -j * 100, 0); // optional layout spacing

                if ((i + j) % 2 != 0)
                    child.transform.GetChild(0).GetComponent<Image>().sprite = pink;

                string label = $"{i}{j}";
                child.normalStateNum.text = label;
                child.name = label;

                Undo.RegisterCreatedObjectUndo(child.gameObject, "Create Block");
            }
        }

        EditorSceneManager.MarkSceneDirty(parent.gameObject.scene);
        Debug.Log("Created 100 children.");
#else
        Debug.LogWarning("Editor-only functionality.");
#endif
    }

    [ContextMenu("Create 10 Children")]
    private void Create10Children()
    {
#if UNITY_EDITOR
        if (!ValidateInput()) return;

        ClearChildren();

        for (int i = 0; i < 10; i++)
        {
            Block child = (Block)PrefabUtility.InstantiatePrefab(prefab, parent.gameObject.scene);
            child.transform.SetParent(parent);
            child.transform.localPosition = new Vector3(i * 100, 0, 0);

            if (i % 2 != 0)
                child.transform.GetChild(0).GetComponent<Image>().sprite = pink;

            string label = $"{i}";
            child.normalStateNum.text = label;
            child.name = label;

            Undo.RegisterCreatedObjectUndo(child.gameObject, "Create Block");
        }

        EditorSceneManager.MarkSceneDirty(parent.gameObject.scene);
        Debug.Log("Created 10 children.");
#else
        Debug.LogWarning("Editor-only functionality.");
#endif
    }



    [ContextMenu("update 100 Children")]
    private void Update100Children()
    {
#if UNITY_EDITOR
        if (!ValidateInput()) return;

        foreach (Transform item in parent)
        {
            string s = item.GetComponent<Block>().normalStateNum.text;
            item.GetComponent<Block>().clickedStateNum.text = s;
        }


        EditorSceneManager.MarkSceneDirty(parent.gameObject.scene);
        Debug.Log("Created 100 children.");
#else
        Debug.LogWarning("Editor-only functionality.");
#endif
    }


    private bool ValidateInput()
    {
        if (parent == null)
        {
            Debug.LogWarning("Assign a parent Transform.");
            return false;
        }

        if (prefab == null)
        {
            Debug.LogWarning("Assign a prefab.");
            return false;
        }

        return true;
    }

    [ContextMenu("Clear children")]
    private void ClearChildren()
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);
#else
            DestroyImmediate(parent.GetChild(i).gameObject);
#endif
        }
    }
}
