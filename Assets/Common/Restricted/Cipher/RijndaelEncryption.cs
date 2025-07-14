using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace DevCommon.Cipher
{
    public class RijndaelEncryption
    {
        private static int bufferKeySize = 32;
        private static int blockSize = 256;
        private static int keySize = 256;

        /// <summary>
        /// <para>If you want to update the settings, you can update the settings.</para>
        /// <para>【argument1】buffer key size</para>
        /// <para>【argument2】block size</para>
        /// <para>【argument3】key size</para>
        /// </summary>
        public static void UpdateEncryptionKeySize(int a_BufferKeySize = 32, int a_BlockSize = 256, int a_KeySize = 256)
        {
            RijndaelEncryption.bufferKeySize = a_BufferKeySize;
            RijndaelEncryption.blockSize = a_BlockSize;
            RijndaelEncryption.keySize = a_KeySize;
        }

        /// <summary>
        /// <para>Standard Rijndael(AES) encrypt</para>
        /// <para>【argument1】plane text</para>
        /// <para>【argument2】password</para>
        /// <para>【return】Encrypted and converted to Base64 string</para>
        /// </summary>
        public static string Encrypt(string a_Plane, string a_Password)
        {
            byte[] t_Encrypted = Encrypt(Encoding.UTF8.GetBytes(a_Plane), a_Password);
            return Convert.ToBase64String(t_Encrypted);
        }

        /// <summary>
        /// <para>Standard Rijndael(AES) encrypt</para>
        /// <para>【argument1】plane binary</para>
        /// <para>【argument2】password</para>
        /// <para>【return】Encrypted binary</para>
        /// </summary>
        public static byte[] Encrypt(byte[] a_Src, string a_Password)
        {
            RijndaelManaged t_Rij = SetupRijndaelManaged;

            // A pseudorandom number is newly generated based on the inputted password
            Rfc2898DeriveBytes t_DeriveBytes = new Rfc2898DeriveBytes(a_Password, bufferKeySize);
            // The missing parts are specified in advance to fill in 0 length
            byte[] t_Salt = new byte[bufferKeySize];
            // Rfc2898DeriveBytes gets an internally generated satl
            t_Salt = t_DeriveBytes.Salt;
            // The 32-byte data extracted from the generated pseudorandom number is used as a password
            byte[] t_BufferKey = t_DeriveBytes.GetBytes(bufferKeySize);

            t_Rij.Key = t_BufferKey;
            t_Rij.GenerateIV();

            using (ICryptoTransform encrypt = t_Rij.CreateEncryptor(t_Rij.Key, t_Rij.IV))
            {
                byte[] t_Dest = encrypt.TransformFinalBlock(a_Src, 0, a_Src.Length);
                // first 32 bytes of salt and second 32 bytes of IV for the first 64 bytes
                List<byte> t_Compile = new List<byte>(t_Salt);
                t_Compile.AddRange(t_Rij.IV);
                t_Compile.AddRange(t_Dest);
                return t_Compile.ToArray();
            }
        }

        /// <summary>
        /// <para>Standard Rijndael(AES) decrypt</para>
        /// <para>【argument1】encrypted string</para>
        /// <para>【argument2】password</para>
        /// <para>【return】Decrypted string</para>
        /// </summary>
        public static string Decrypt(string t_Encrtpted, string t_Password)
        {
            byte[] t_Decripted = Decrypt(Convert.FromBase64String(t_Encrtpted), t_Password);
            return Encoding.UTF8.GetString(t_Decripted);
        }

        /// <summary>
        /// <para>Standard Rijndael(AES) decrypt</para>
        /// <para>【argument1】encrypted binary</para>
        /// <para>【argument2】password</para>
        /// <para>【return】Decrypted binary</para>
        /// </summary>
        public static byte[] Decrypt(byte[] t_Src, string t_Password)
        {
            RijndaelManaged t_Rij = SetupRijndaelManaged;

            List<byte> t_Compile = new List<byte>(t_Src);

            // First 32 bytes are salt.
            List<byte> t_Salt = t_Compile.GetRange(0, bufferKeySize);
            // Second 32 bytes are IV.
            List<byte> t_IV = t_Compile.GetRange(bufferKeySize, bufferKeySize);
            t_Rij.IV = t_IV.ToArray();

            Rfc2898DeriveBytes t_DeriveBytes = new Rfc2898DeriveBytes(t_Password, t_Salt.ToArray());
            byte[] t_BufferKey = t_DeriveBytes.GetBytes(bufferKeySize);    // Convert 32 bytes of salt to password
            t_Rij.Key = t_BufferKey;

            byte[] t_Plain = t_Compile.GetRange(bufferKeySize * 2, t_Compile.Count - (bufferKeySize * 2)).ToArray();

            using (ICryptoTransform decrypt = t_Rij.CreateDecryptor(t_Rij.Key, t_Rij.IV))
            {
                byte[] t_Dest = decrypt.TransformFinalBlock(t_Plain, 0, t_Plain.Length);
                return t_Dest;
            }
        }

        private static RijndaelManaged SetupRijndaelManaged
        {
            get
            {
                RijndaelManaged t_Rij = new RijndaelManaged();
                t_Rij.BlockSize = blockSize;
                t_Rij.KeySize = keySize;
                t_Rij.Mode = CipherMode.CBC;
                t_Rij.Padding = PaddingMode.PKCS7;
                return t_Rij;
            }
        }
    }
}