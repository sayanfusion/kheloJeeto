using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace DevCommon.Utils
{
    public sealed class CommonFunctions
    {
        // Get the exact value for angle along the Y axis using the difference between two positions
        public static float GetFloatAngleAlongYAxis(Vector3 position, Vector3 targetPosition, int angleOffset = 0)
        {
            Vector3 targetDir = targetPosition - position;
            targetDir.y = 0;
            targetDir = Vector3.Normalize(targetDir);
            float angle = (float)(Mathf.Atan2(1, 0) - Mathf.Atan2(targetDir.z, targetDir.x)) * Mathf.Rad2Deg;
            return angle + angleOffset;
        }

        // Returns the angle along the Y axis using the difference between two positions
        public static int GetAngleAlongYAxis(Vector3 position, Vector3 targetPosition, int angleOffset = 0, int angleSnap = 90)
        {
            Vector3 targetDir = targetPosition - position;
            targetDir.y = 0;
            targetDir = Vector3.Normalize(targetDir);
            float angle = (float)(Mathf.Atan2(1, 0) - Mathf.Atan2(targetDir.z, targetDir.x)) * Mathf.Rad2Deg;

            int angleInt = Mathf.RoundToInt(angle);
            angleInt = Mathf.RoundToInt(angleInt / (float)angleSnap);
            angleInt *= angleSnap;
            return angleInt + angleOffset;
        }

        // Rotate an object along Y axis to a particular angle
        public static void SetRotationOnYAxis(Transform a_transform, float angle)
        {
            Vector3 rot = a_transform.localEulerAngles;
            rot.y = angle;
            a_transform.localEulerAngles = rot;
        }

        // Rotate an object along Y axis towards a target
        public static void RotateObjectToFaceAlongYAxis(Transform a_transform, Vector3 targetPosition, int angleOffset = 0, int clampAngle = 1)
        {
            float angle = GetAngleAlongYAxis(a_transform.position, targetPosition, angleOffset);
            int requiredAngle = (int)((angle / clampAngle) * clampAngle);
            SetRotationOnYAxis(a_transform, requiredAngle);
        }

        // Sets the layer property to all child objects in a transform and sub objects
        public static void SetLayer(GameObject obj, int newLayer, bool recursively = true)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                if (child == null)
                    continue;

                if (recursively)
                    CommonFunctions.SetLayer(child.gameObject, newLayer, recursively);
            }
        }

        // Check if mask contains this value
        public static bool MaskContainsValue<T>(T mask, T val)
        {
            int maskValue = (int)(object)mask;
            int flagValue = (int)(object)val;
            return ((maskValue & flagValue) != 0);
        }

        // Get the total masked bit value
        public static int GetTotalMaskBitValue<T>(T[] arr)
        {
            int maskValue = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                maskValue += (int)(object)arr[i];
            }
            return maskValue;
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }

        // Get object's forward facing angle
        public static float GetPlayerForwardFacingAngle(Transform objTransform)
        {
            Vector3 forward = objTransform.forward;
            forward.y = 0;
            float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
            return headingAngle;
        }

        // Gives back a rotated texture
        public static Texture2D RotateTexture(Texture2D tex, float phi)
        {
            int x;
            int y;
            int i;
            int j;
            float sn = Mathf.Sin(phi);
            float cs = Mathf.Cos(phi);
            Color32[] arr = tex.GetPixels32();
            Color32[] arr2 = tex.GetPixels32();
            int W = tex.width;
            int H = tex.height;
            int xc = W / 2;
            int yc = H / 2;

            for (j = 0; j < H; j++)
            {
                for (i = 0; i < W; i++)
                {
                    arr2[j * W + i] = new Color32(0, 0, 0, 0);

                    x = Mathf.RoundToInt(cs * (i - xc) + sn * (j - yc) + xc);
                    y = Mathf.RoundToInt(-sn * (i - xc) + cs * (j - yc) + yc);

                    if ((x > -1) && (x < W) && (y > -1) && (y < H))
                    {
                        arr2[j * W + i] = arr[y * W + x];
                    }
                }
            }

            Texture2D newTexture = new Texture2D(W, H);
            newTexture.SetPixels32(arr2);
            return newTexture;
        }

        public static T GetUiRaycastElement<T>(Vector2 a_RayOriginPositon)
        {
            PointerEventData t_PointerData = new PointerEventData(EventSystem.current) { pointerId = -1 };
            t_PointerData.position = a_RayOriginPositon;
            List<RaycastResult> t_Results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(t_PointerData, t_Results);
            RaycastResult t_Result = t_Results.Where(x => x.gameObject.GetComponent<T>() != null).FirstOrDefault();
            return t_Result.gameObject != null ? t_Result.gameObject.GetComponent<T>() : default;
        }

        public static List<RaycastResult> GetUiRaycastElements(Vector2 a_RayOriginPositon)
        {
            PointerEventData t_PointerData = new PointerEventData(EventSystem.current) { pointerId = -1 };
            t_PointerData.position = a_RayOriginPositon;
            List<RaycastResult> t_Results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(t_PointerData, t_Results);
            return t_Results;
        }

        // Sets the layer of all child objects in game object to the same layer
        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null)
            {
                return;
            }

            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                if (child == null)
                    continue;

                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        public static void ReplaceSpacesAndTabs(string originalData, string replaceWith)
        {
            string line = originalData.Replace("\t", replaceWith);
            while (line.IndexOf("  ") >= 0)
            {
                line = line.Replace("  ", replaceWith);
            }
        }

        public static void ChangeScreenOrientation(ScreenOrientation screenOrientation)
        {
            switch (screenOrientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    Screen.autorotateToPortrait = false;
                    Screen.autorotateToLandscapeLeft = true;
                    Screen.autorotateToPortraitUpsideDown = false;
                    Screen.autorotateToLandscapeLeft = true;
                    break;

                case ScreenOrientation.Portrait:
                    Screen.autorotateToPortrait = false;
                    Screen.autorotateToLandscapeLeft = true;
                    Screen.autorotateToPortraitUpsideDown = false;
                    Screen.autorotateToLandscapeLeft = true;
                    break;
            }
            Screen.orientation = screenOrientation;
        }

        public static List<GameObject> GetDontDestroyOnLoadObjects()
        {
            List<GameObject> result = new List<GameObject>();

            List<GameObject> rootGameObjectsExceptDontDestroyOnLoad = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                rootGameObjectsExceptDontDestroyOnLoad.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
            }

            List<GameObject> rootGameObjects = new List<GameObject>();
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < allTransforms.Length; i++)
            {
                Transform root = allTransforms[i].root;
                if (root.hideFlags == HideFlags.None && !rootGameObjects.Contains(root.gameObject))
                {
                    rootGameObjects.Add(root.gameObject);
                }
            }

            for (int i = 0; i < rootGameObjects.Count; i++)
            {
                if (!rootGameObjectsExceptDontDestroyOnLoad.Contains(rootGameObjects[i]))
                    result.Add(rootGameObjects[i]);
            }

            //foreach( GameObject obj in result )
            //    Debug.Log( obj );

            return result;
        }

        // Get random alphanumeric unique value
        // Generating same randome value more than once is not possible
        public static string GetUniqueKey(int a_Size)
        {
            char[] t_Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] t_Data = new byte[4 * a_Size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(t_Data);
            }

            StringBuilder t_Result = new StringBuilder(a_Size);
            for (int i = 0; i < a_Size; i++)
            {
                var t_Rnd = BitConverter.ToUInt32(t_Data, i * 4);
                var t_Idx = t_Rnd % t_Chars.Length;

                t_Result.Append(t_Chars[t_Idx]);
            }

            return t_Result.ToString();
        }
    }

    public class OrderedDictionary<T, K>
    {
        private Dictionary<T, K> baseDictionary;
        private Dictionary<long, T> entryTimeDictionary;

        public OrderedDictionary()
        {
            this.baseDictionary = new Dictionary<T, K>();
        }

        public void Add(T key, K val)
        {
            this.baseDictionary[key] = val;
            this.entryTimeDictionary[DateTime.Now.Ticks] = key;
        }

        public List<KeyValuePair<T, K>> GetLastEnteredItems(int numberOfEntries)
        {
            // Find n last keys.
            var lastEntries =
                this.entryTimeDictionary
                    .OrderByDescending(i => i.Key)
                    .Take(numberOfEntries)
                    .Select(i => i.Value);

            // Return KeyValuePair for itmes with last n keys
            return this.baseDictionary
                       .Where(i => lastEntries.Contains(i.Key))
                       .ToList();
        }

        public bool Any()
        {
            return baseDictionary.Any();
        }

        public int Count()
        {
            return baseDictionary.Count();
        }
    }
}