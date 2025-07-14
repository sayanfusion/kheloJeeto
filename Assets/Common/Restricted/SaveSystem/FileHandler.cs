using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEditor;

namespace DevCommon.SaveSystem
{
    public class FileHandler
    {
        // Reads a object from file in specified path
        public static bool ReadFromFile<T>(string a_FilePath, out T a_Data)
        {
            a_Data = default;
            string t_FileLocation = Application.persistentDataPath + "/" + a_FilePath;

            if (File.Exists(t_FileLocation))
            {
                BinaryFormatter t_BinaryFormatter = new BinaryFormatter();
                FileStream t_File = File.Open(t_FileLocation, FileMode.Open);
                string t_ReadString = (string)t_BinaryFormatter.Deserialize(t_File);
                t_File.Close();
                a_Data = JsonConvert.DeserializeObject<T>(t_ReadString);
                return true;
            }
            else
                return false;
        }

        // Reads a string from file in specified path
        public static bool ReadFromFile(string a_FilePath, out string a_Data)
        {
            a_Data = default;
            string t_FileLocation = Application.persistentDataPath + "/" + a_FilePath;

            if (File.Exists(t_FileLocation))
            {
                BinaryFormatter t_BinaryFormatter = new BinaryFormatter();
                FileStream t_File = File.Open(t_FileLocation, FileMode.Open);
                a_Data = (string)t_BinaryFormatter.Deserialize(t_File);
                t_File.Close();
                return true;
            }
            else
                return false;
        }

        // Saves a file with given string content to specified path
        public static void SaveToFile(string a_Data, string a_FilePath, FileMode a_FileMode = FileMode.OpenOrCreate)
        {
            string t_FileLocation = Application.persistentDataPath + "/" + a_FilePath;
            BinaryFormatter t_BinaryFormatter = new BinaryFormatter();
            FileStream t_File = File.Open(t_FileLocation, a_FileMode);
            t_BinaryFormatter.Serialize(t_File, a_Data);
            t_File.Close();
        }

        // Saves a file with given object content to specified path
        public static void SaveToFile<T>(T a_Data, string a_FilePath, FileMode a_FileMode = FileMode.OpenOrCreate)
        {
            string t_FileLocation = Application.persistentDataPath + "/" + a_FilePath;
            BinaryFormatter t_BinaryFormatter = new BinaryFormatter();
            FileStream t_File = File.Open(t_FileLocation, a_FileMode);
            t_BinaryFormatter.Serialize(t_File, JsonConvert.SerializeObject(a_Data));
            t_File.Close();
        }

        // Delete a file from specified path
        public static bool DeleteSaveFile(string a_FilePath)
        {
            string t_FileLocation = Application.persistentDataPath + "/" + a_FilePath;
            try
            {
                File.Delete(t_FileLocation);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        // Saves a key with given string content to player prefs
        public static void SaveToPlayerPrefs(string a_Key, string a_Value)
        {
            PlayerPrefs.SetString(a_Key, a_Value);
        }

        // Reads a string from key in player prefs
        public static bool ReadFromPlayerPrefs(string a_Key, out string a_Value, string a_DefaultValue = "")
        {
            a_Value = string.Empty;
            if (PlayerPrefs.HasKey(a_Key))
            {
                a_Value = PlayerPrefs.GetString(a_Key, a_DefaultValue);
                return true;
            }
            else
                return false;
        }

        // Delete a key from player prefs
        public static bool DeletePlayerPrefsKey(string a_Key)
        {
            if (PlayerPrefs.HasKey(a_Key))
            {
                PlayerPrefs.DeleteKey(a_Key);
                return true;
            }
            else
                return false;
        }

        // Delete all keys from player prefs
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

#if UNITY_EDITOR
        [MenuItem("DevCommon/Clear PlayerPrefs")]
        private static void cleaAllrPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
#endif
    }
}