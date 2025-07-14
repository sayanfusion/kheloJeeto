using System.IO;
using UnityEngine;
using DevCommon.Cipher;
using Newtonsoft.Json;

namespace DevCommon.SaveSystem
{
    public enum EncryptionMode
    {
        Rijndael,
        RSA
    }

    public class SecureFileHandler : MonoBehaviour
    {
        // Reads a secure object from file in specified path
        public static bool ReadFromFile<T>(string a_FilePath, out T a_Data, string a_Password, EncryptionMode a_EncryptionMode = EncryptionMode.Rijndael)
        {
            a_Data = default(T);

            if (FileHandler.ReadFromFile(a_FilePath, out string t_EncryptedData))
            {
                string t_DecryptedData = a_EncryptionMode == EncryptionMode.Rijndael ? RijndaelEncryption.Decrypt(t_EncryptedData, a_Password) : RSAEncryption.Decrypt(t_EncryptedData, a_Password);
                a_Data = JsonConvert.DeserializeObject<T>(t_DecryptedData);
                return true;
            }
            else
                return false;
        }

        // Reads a secure string from file in specified path
        public static bool ReadFromFile(string a_FilePath, out string a_Data, string a_Password, EncryptionMode a_EncryptionMode = EncryptionMode.Rijndael)
        {
            a_Data = string.Empty;

            if (FileHandler.ReadFromFile(a_FilePath, out string t_EncryptedData))
            {
                a_Data = a_EncryptionMode == EncryptionMode.Rijndael ? RijndaelEncryption.Decrypt(t_EncryptedData, a_Password) : RSAEncryption.Decrypt(t_EncryptedData, a_Password);
                return true;
            }
            else
                return false;
        }

        // Saves a secure file with given string content to specified path
        public static void SaveToFile(string a_Data, string a_FilePath, string a_Password, EncryptionMode a_EncryptionMode = EncryptionMode.Rijndael, FileMode a_FileMode = FileMode.Create)
        {
            string t_EncryptedData = a_EncryptionMode == EncryptionMode.Rijndael ? RijndaelEncryption.Encrypt(a_Data, a_Password) : RSAEncryption.Encrypt(a_Data, a_Password);
            FileHandler.SaveToFile(t_EncryptedData, a_FilePath, a_FileMode);
        }

        // Saves a secure file with given object content to specified path
        public static void SaveToFile<T>(T a_Data, string a_FilePath, string a_Password, EncryptionMode a_EncryptionMode = EncryptionMode.Rijndael, FileMode a_FileMode = FileMode.Create)
        {
            string t_StrData = JsonConvert.SerializeObject(a_Data);
            string t_EncryptedData = a_EncryptionMode == EncryptionMode.Rijndael ? RijndaelEncryption.Encrypt(t_StrData, a_Password) : RSAEncryption.Encrypt(t_StrData, a_Password);
            FileHandler.SaveToFile(t_EncryptedData, a_FilePath, a_FileMode);
        }

        // Saves a secure key with given string content to player prefs
        public static void SaveToPlayerPrefs(string a_Key, string a_Value, string a_Password, EncryptionMode a_EncryptionMode = EncryptionMode.Rijndael)
        {
            string t_EncryptedValue = a_EncryptionMode == EncryptionMode.Rijndael ? RijndaelEncryption.Encrypt(a_Value, a_Password) : RSAEncryption.Encrypt(a_Value, a_Password);
            PlayerPrefs.SetString(a_Key, t_EncryptedValue);
        }

        // Reads a secure string from key in player prefs
        public static bool ReadFromPlayerPrefs(string a_Key, out string a_Value, string a_Password, EncryptionMode a_EncryptionMode = EncryptionMode.Rijndael, string a_DefaultValue = "")
        {
            a_Value = string.Empty;

            if (PlayerPrefs.HasKey(a_Key))
            {
                string t_EncryptedValue = PlayerPrefs.GetString(a_Key, a_DefaultValue);
                a_Value = a_EncryptionMode == EncryptionMode.Rijndael ? RijndaelEncryption.Decrypt(t_EncryptedValue, a_Password) : RSAEncryption.Decrypt(t_EncryptedValue, a_Password);
                return true;
            }
            else
                return false;
        }
    }
}