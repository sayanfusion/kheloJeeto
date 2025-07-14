using UnityEngine;
using System.IO;
using System;

namespace DevCommon.SaveSystem
{
    public class ImageHandler
    {
        // Save image data
        public static void SaveImage(string a_FilePath, byte[] a_ImageBytes)
        {
            string t_FileLocation = Application.persistentDataPath + "/" + a_FilePath;
            Debug.Log(t_FileLocation);

            if (!Directory.Exists(Path.GetDirectoryName(t_FileLocation)))
                Directory.CreateDirectory(Path.GetDirectoryName(t_FileLocation));

            File.WriteAllBytes(a_FilePath, a_ImageBytes);
        }

        // Get image data in Texture2D
        public static bool GetImageTexture2D(string a_FilePath, out Texture2D a_Texture2D)
        {
            string t_FileLocation = Application.persistentDataPath + "/" + a_FilePath;

            a_Texture2D = new Texture2D(2, 2);
            if (!Directory.Exists(Path.GetDirectoryName(t_FileLocation)) || !File.Exists(t_FileLocation))
            {
                Debug.LogError("File does not exist!");
                return false;
            }
            else
            {
                GetImageBytes(t_FileLocation, out byte[] t_DataByte);
                a_Texture2D.LoadImage(t_DataByte);
                return true;
            }
        }

        // Get image data in bytes
        public static bool GetImageBytes(string a_FilePath, out byte[] a_ImageBytes)
        {
            string t_FileLocation = Application.persistentDataPath + "/" + a_FilePath;

            a_ImageBytes = default;
            if (!Directory.Exists(Path.GetDirectoryName(t_FileLocation)) || !File.Exists(t_FileLocation))
            {
                Debug.LogError("File does not exist!");
                return false;
            }
            else
            {
                a_ImageBytes = File.ReadAllBytes(t_FileLocation);
                return true;
            }
        }

        // Convert string to Texture2D 
        public static Texture2D StringToTexture2D(string a_Data)
        {
            byte[] t_ImageBytes = Convert.FromBase64String(a_Data);
            Texture2D t_Tex = new Texture2D(2, 2);
            t_Tex.LoadImage(t_ImageBytes);
            return t_Tex;
        }

        // Convert bytes to Texture2D 
        public static Texture2D BytesToTexture2D(byte[] a_Data)
        {
            byte[] t_ImageBytes = a_Data;
            Texture2D t_Tex = new Texture2D(2, 2);
            t_Tex.LoadImage(t_ImageBytes);
            return t_Tex;
        }
    }
}