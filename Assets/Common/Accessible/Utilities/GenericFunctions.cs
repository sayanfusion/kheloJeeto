using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DevCommon.Utils
{
    public static class GenericFunctions
    {
        // Returns a cloned object
        public static T Clone<T>(T obj)
        {
            string jsonStr = JsonUtility.ToJson(obj);
            return JsonUtility.FromJson<T>(jsonStr);
        }

        // Returns a cloned object
        public static void Clone<T>(T obj, out T copyTo)
        {
            string jsonStr = JsonUtility.ToJson(obj);
            copyTo = JsonUtility.FromJson<T>(jsonStr);
        }

        // Auto map properties/values from one object to another
        public static void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);
            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }

        public static bool ValidateEmail(string email)
        {
            Regex mailValidator = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$");
            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                return false;
            if (mailValidator.IsMatch(email))
                return true;
            else
                return false;
        }

        public static bool SpecialCharacterCheck(string password, int reqAmount)
        {
            bool accepatble = false;
            int count = password.Where(c => !char.IsLetterOrDigit(c)).Count();
            if (count >= reqAmount) accepatble = true;
            return accepatble;
        }

        public static bool AlphabateCheck(string password, int reqAmount)
        {
            bool accepatble = false;
            int count = password.Where(x => char.IsLetter(x)).Count();
            if (count >= reqAmount) accepatble = true;
            return accepatble;
        }

        public static bool NumberCheck(string password, int reqAmount)
        {
            bool accepatble = false;
            int count = password.Where(x => char.IsDigit(x)).Count();
            if (count >= reqAmount) accepatble = true;
            return accepatble;
        }

        public static Texture2D CalculateCircleTexture(int reqHeight, int reqWidth, float radius, float centerX, float centerY, Texture2D sourceTex)
        {
            Color[] colorData = sourceTex.GetPixels(0, 0, sourceTex.width, sourceTex.height);
            Texture2D texture = new Texture2D(reqHeight, reqWidth);
            for (int i = (int)(centerX - radius); i < centerX + radius; i++)
            {
                for (int j = (int)(centerY - radius); j < centerY + radius; j++)
                {
                    float dx = i - centerX;
                    float dy = j - centerY;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    if (d <= radius)
                        texture.SetPixel(i - (int)(centerX - radius), j - (int)(centerY - radius), sourceTex.GetPixel(i, j));
                    else
                        texture.SetPixel(i - (int)(centerX - radius), j - (int)(centerY - radius), Color.clear);
                }
            }
            texture.Apply();
            return texture;
        }

        public static float Remap(this float a_Value, float a_From1, float a_To1, float a_From2, float a_To2) => (a_Value - a_From1) / (a_To1 - a_From1) * (a_To2 - a_From2) + a_From2;
    }

    public static class ListExt
    {
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static List<List<T>> SplitList<T>(this List<T> a_TargetList, int a_RequiredSize)
        {
            List<List<T>> t_List = new List<List<T>>();
            for (int i = 0; i < a_TargetList.Count; i += a_RequiredSize)
                t_List.Add(a_TargetList.GetRange(i, System.Math.Min(a_RequiredSize, a_TargetList.Count - i)));
            return t_List;
        }

        public static IEnumerable<T> GetElementsInRandomOrder<T>(IEnumerable<T> a_TargetList, int a_NoOfElements)
        {
            System.Random random = new System.Random(DateTime.Now.Millisecond);
            Dictionary<double, T> randomSortTable = new Dictionary<double, T>();

            foreach (T someType in a_TargetList)
                randomSortTable[random.NextDouble()] = someType;

            return randomSortTable.OrderBy(x => x.Key).Take(a_NoOfElements).Select(x => x.Value);
        }

        public static List<T> GetDistinctRandom<T>(this IList<T> source, int count)
        {
            if (count > source.Count)
                throw new ArgumentOutOfRangeException();

            if (count == source.Count)
                return new List<T>(source);

            var sourceDict = source.ToIndexedDictionary();

            if (count > source.Count / 2)
            {
                while (sourceDict.Count > count)
                    sourceDict.Remove(source.GetRandomIndex());

                return sourceDict.Select(kvp => kvp.Value).ToList();
            }

            var randomDict = new Dictionary<int, T>(count);
            while (randomDict.Count < count)
            {
                int key = source.GetRandomIndex();
                if (!randomDict.ContainsKey(key))
                    randomDict.Add(key, sourceDict[key]);
            }

            return randomDict.Select(kvp => kvp.Value).ToList();
        }

        static System.Random rnd = new System.Random();
        public static int GetRandomIndex<T>(this ICollection<T> source)
        {
            return rnd.Next(source.Count);
        }

        public static T GetRandom<T>(this IList<T> source)
        {
            return source[source.GetRandomIndex()];
        }

        static void AddRandomly<T>(this ICollection<T> toCol, IList<T> fromList, int count)
        {
            while (toCol.Count < count)
                toCol.Add(fromList.GetRandom());
        }

        public static Dictionary<int, T> ToIndexedDictionary<T>(this IEnumerable<T> lst)
        {
            return lst.ToIndexedDictionary(t => t);
        }

        public static Dictionary<int, T> ToIndexedDictionary<S, T>(this IEnumerable<S> lst,
                                                                   Func<S, T> valueSelector)
        {
            int index = -1;
            return lst.ToDictionary(t => ++index, valueSelector);
        }
    }

    public static class EnumExt
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }

        public static T Parse<T>(this Enum theEnum, string valueToParse)
        {
            T returnValue = default(T);
            if (Enum.IsDefined(typeof(T), valueToParse))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                returnValue = (T)converter.ConvertFromString(valueToParse);
            }
            return returnValue;
        }

        public static bool TryParse<T>(this Enum theEnum, string valueToParse, out T returnValue)
        {
            returnValue = default(T);
            if (Enum.IsDefined(typeof(T), valueToParse))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                returnValue = (T)converter.ConvertFromString(valueToParse);
                return true;
            }
            return false;
        }

        // Get enum data as a string List
        public static List<string> GetListOfEnums(Type enumType)
        {
            List<string> enumList = Enum.GetNames(enumType).ToList<string>();
            return enumList;
        }

        public static object EnumFromString(Type enumType, string value)
        {
            return Enum.Parse(enumType, value);
        }
    }

    public static class LinqExt
    {
        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the default equality comparer for the projected type.
        /// </summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the specified comparer for the projected type.
        /// </summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return _(); IEnumerable<TSource> _()
            {
                var knownKeys = new HashSet<TKey>(comparer);
                foreach (var element in source)
                {
                    if (knownKeys.Add(keySelector(element)))
                        yield return element;
                }
            }
        }

        //This avoids the hassle of writing the Equals operator from our side every time.
        public static bool Contains<S, T>(this IEnumerable<S> lstObject, Func<S, T> comparer, T valueToMatch)
        {
            return lstObject.Any(s => comparer(s).Equals(valueToMatch));
        }

        public static bool Add<S, T>(this ICollection<S> lstObject, Func<S, T> selector, S objectToAdd)
        {
            //Example:- List.Add(x => x, this);
            if (lstObject.Contains(selector, selector(objectToAdd)))
                return false;

            lstObject.Add(objectToAdd);
            return true;
        }
    }

    public class TimeUtils
    {
        private static readonly DateTime Jan1st1970 = new DateTime
        (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }

    public static class UnityObjectExt
    {
        #region Custom GetComponent & HasComponent For GameObject Element
        public static TComponent CGetComponent<TComponent, KIgnore>(this GameObject targetObject)
        {
            if (targetObject.GetComponent<KIgnore>() == null)
                return targetObject.GetComponent<TComponent>();
            else
                return default;
        }

        public static List<TComponent> CGetComponentsInChildren<TComponent, KIgnore>(this GameObject targetObject) where TComponent : UnityEngine.Component where KIgnore : UnityEngine.Component
        {
            List<TComponent> t_Components = targetObject.GetComponentsInChildren<TComponent>().ToList();
            t_Components.RemoveAll(x => x.GetComponent<KIgnore>() != null);
            return t_Components;
        }

        public static bool CHasComponent<TComponent>(this GameObject targetObject) => targetObject.GetComponent<TComponent>() != null ? true : false;
        #endregion

        #region Custom GetComponent & HasComponent For Component Element
        public static TComponent CGetComponent<TComponent, KIgnore>(this UnityEngine.Component targetObject) where TComponent : UnityEngine.Component where KIgnore : UnityEngine.Component
        {
            if (targetObject.GetComponent<KIgnore>() == null)
                return targetObject.GetComponent<TComponent>();
            else
                return default;
        }

        public static List<TComponent> CGetComponentsInChildren<TComponent, KIgnore>(this UnityEngine.Component targetObject) where TComponent : UnityEngine.Component where KIgnore : UnityEngine.Component
        {
            List<TComponent> t_Components = targetObject.GetComponentsInChildren<TComponent>().ToList();
            t_Components.RemoveAll(x => x.GetComponent<KIgnore>() != null);
            return t_Components;
        }

        public static bool CHasComponent<TComponent>(this UnityEngine.Component targetObject) where TComponent : UnityEngine.Component => targetObject.GetComponent<TComponent>() != null ? true : false;
        #endregion

        // Replace all whitespaces
        public static string NonWhitespace(this string targetString, string a_ReplaceWith = "")
        {
            return Regex.Replace(targetString, @"\s+", a_ReplaceWith);
        }

        // Replace all special characters and numbers
        public static string LetterOnly(this string targetString, string a_ReplaceWith = "")
        {
            return Regex.Replace(targetString, "[^a-zA-Z]+", a_ReplaceWith);
            //return Regex.Replace(targetString, "[^0-9a-zA-Z]+", a_ReplaceWith);
        }
    }
}