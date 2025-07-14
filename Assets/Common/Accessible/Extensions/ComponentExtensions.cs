using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DevCommon
{
    public static class ComponentExtensions
    {
        public static T Spawn<T>(this GameObject prefab) where T : Component
        {
            return Object.Instantiate(prefab).GetComponent<T>();
        }

        public static T Spawn<T>(this GameObject prefab, Transform parent, bool worldPositionStays = false)
            where T : Component
        {
            var instance = prefab.Spawn<T>();
            if (!worldPositionStays)
            {
                instance.transform.SetParent(parent, worldPositionStays);
            }
            else
            {
                Transform transform;
                (transform = instance.transform).SetParent(parent);
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
                transform.localScale = Vector3.one;
            }

            return instance;
        }

        public static void SetActive(this Component behaviour, bool value)
        {
            behaviour.gameObject.SetActive(value);
        }

        public static void ClearChildren(this Transform transform)
        {
            foreach (Transform child in transform)
                Object.Destroy(child.gameObject);
        }

        public static Toggle GetActive(this ToggleGroup aGroup)
        {
            return aGroup.ActiveToggles().FirstOrDefault();
        }

        public static void DisableFirstChild(this Transform transform)
        {
            if (transform.childCount > 0)
                transform.GetChild(0).SetActive(false);
        }

        public static void Toggle(this CanvasGroup canvasGroup, bool value)
        {
            canvasGroup.alpha = value ? 1 : 0;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }
    }
}