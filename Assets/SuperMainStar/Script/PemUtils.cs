using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public static class PemUtils
{
    public static bool IsWindows7()
    {
        return (System.Environment.OSVersion.Version.Major == 6 && System.Environment.OSVersion.Version.Minor == 1);
    }
    public static string LoadPublicKeyFromPem(string pemFilePath)
    {
        string pemContent = File.ReadAllText(pemFilePath);
        Debug.Log("PEM Content : " + pemContent);
        string publicKey = GetPublicKey(pemContent);
        return publicKey;
    }

    private static string GetPublicKey(string pemContent)
    {
        string header = "-----BEGIN PUBLIC KEY-----";
        string footer = "-----END PUBLIC KEY-----";

        int start = pemContent.IndexOf(header, StringComparison.Ordinal);
        if (start < 0)
        {
            throw new ArgumentException("Header not found in PEM content.");
        }

        start += header.Length;

        int end = pemContent.IndexOf(footer, start, StringComparison.Ordinal);
        if (end < 0)
        {
            throw new ArgumentException("Footer not found in PEM content.");
        }

        string base64 = pemContent.Substring(start, end - start);

        string pkey = Regex.Replace(base64, @"\s+", "");
        Debug.Log("Public key :" + pkey);
        return pkey;
    }
}
public class AcceptAllCertificatesHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Always return true to accept all certificates
        return true;
    }
}

class AcceptAllCertificatesSignedWithASpecificKeyPublicKey : CertificateHandler
{
    private static string PUB_KEY;

    public AcceptAllCertificatesSignedWithASpecificKeyPublicKey(string pemFilePath)
    {
        PUB_KEY = PemUtils.LoadPublicKeyFromPem(pemFilePath);
    }

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        X509Certificate2 certificate = new X509Certificate2(certificateData);
        string pk = certificate.GetPublicKeyString();
        if (pk.Equals(PUB_KEY))
            return true;

        // Bad dog
        return false;
    }
}
