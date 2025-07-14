using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace DevCommon.Cipher
{
    public class RSAEncryption
    {
        /// <summary>
        /// <para>Generate Public And Private KeyPair</para>
        /// <para>【argument1】keySize</para>
        /// <para>【return】Public key and private key KeyValuePair</para>
        /// </summary>
        public static KeyValuePair<string, string> GenrateKeyPair(int a_KeySize)
        {
            RSACryptoServiceProvider t_RSA = new RSACryptoServiceProvider(a_KeySize);
            string t_PublicKey = t_RSA.ToXmlString(false);
            string t_PrivateKey = t_RSA.ToXmlString(true);
            return new KeyValuePair<string, string>(t_PublicKey, t_PrivateKey);
        }

        /// <summary>
        /// <para>Standard Rijndael(AES) encrypt</para>
        /// <para>【argument1】plane text</para>
        /// <para>【argument2】password</para>
        /// <para>【return】Encrypted and converted to Base64 string</para>
        /// </summary>
        public static string Encrypt(string a_Plane, string a_PublicKey)
        {
            byte[] t_Encrypted = Encrypt(Encoding.UTF8.GetBytes(a_Plane), a_PublicKey);
            return Convert.ToBase64String(t_Encrypted);
        }

        /// <summary>
        /// <para>Standard Rijndael(AES) encrypt</para>
        /// <para>【argument1】plane binary</para>
        /// <para>【argument2】password</para>
        /// <para>【return】Encrypted binary</para>
        /// </summary>
        public static byte[] Encrypt(byte[] a_Src, string a_PublicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(a_PublicKey);
                byte[] t_Encrypted = rsa.Encrypt(a_Src, false);
                return t_Encrypted;
            }
        }

        /// <summary>
        /// <para>Standard Rijndael(AES) decrypt</para>
        /// <para>【argument1】encrypted string</para>
        /// <para>【argument2】password</para>
        /// <para>【return】Decrypted string</para>
        /// </summary>
        public static string Decrypt(string a_Encrtpted, string a_PrivateKey)
        {
            byte[] t_Decripted = Decrypt(Convert.FromBase64String(a_Encrtpted), a_PrivateKey);
            return Encoding.UTF8.GetString(t_Decripted);
        }

        /// <summary>
        /// <para>Standard Rijndael(AES) decrypt</para>
        /// <para>【argument1】encrypted binary</para>
        /// <para>【argument2】password</para>
        /// <para>【return】Decrypted binary</para>
        /// </summary>
        public static byte[] Decrypt(byte[] a_Src, string a_PrivateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(a_PrivateKey);
                byte[] t_Decrypted = rsa.Decrypt(a_Src, false);
                return t_Decrypted;
            }
        }
    }
}