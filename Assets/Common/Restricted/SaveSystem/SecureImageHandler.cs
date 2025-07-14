using System.Collections;
using System.Collections.Generic;
using DevCommon.Cipher;
using UnityEngine;

namespace DevCommon.SaveSystem
{
    public class SecureImageHandler
    {
        // Save secure image data
        public static void SaveImage(string a_FilePath, byte[] a_ImageBytes, string a_Password, EncryptionMode a_EncryptionMode = EncryptionMode.Rijndael)
        {
            byte[] t_EncryptedData = a_EncryptionMode == EncryptionMode.Rijndael ? RijndaelEncryption.Encrypt(a_ImageBytes, a_Password) : RSAEncryption.Encrypt(a_ImageBytes, a_Password);
            ImageHandler.SaveImage(a_FilePath, t_EncryptedData);
        }

        // Get secure image data in Texture2D
        public static bool GetImageTexture2D(string a_FilePath, out Texture2D a_Texture2D, string a_Password, EncryptionMode a_EncryptionMode = EncryptionMode.Rijndael)
        {
            a_Texture2D = new Texture2D(2, 2);
            if (ImageHandler.GetImageBytes(a_FilePath, out byte[] t_EncryptedData))
            {
                byte[] t_DecryptedData = a_EncryptionMode == EncryptionMode.Rijndael ? RijndaelEncryption.Decrypt(t_EncryptedData, a_Password) : RSAEncryption.Decrypt(t_EncryptedData, a_Password);
                a_Texture2D.LoadImage(t_DecryptedData);
                return true;
            }
            else
            {
                return false;
            }
        }

        // Get secure image data in bytes
        public static bool GetImageBytes(string a_FilePath, out byte[] a_ImageBytes, string a_Password, EncryptionMode a_EncryptionMode = EncryptionMode.Rijndael)
        {
            a_ImageBytes = default;
            if (ImageHandler.GetImageBytes(a_FilePath, out byte[] t_EncryptedData))
            {
                a_ImageBytes = a_EncryptionMode == EncryptionMode.Rijndael ? RijndaelEncryption.Decrypt(t_EncryptedData, a_Password) : RSAEncryption.Decrypt(t_EncryptedData, a_Password);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}