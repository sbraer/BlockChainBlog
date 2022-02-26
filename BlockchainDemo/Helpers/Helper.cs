using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainDemo.Helpers;

public static class Helper
{
    public static byte[] HashBytes(this byte[] content)
    {
        using var sha256Hash = SHA256.Create();
        return sha256Hash.ComputeHash(sha256Hash.ComputeHash(content));
    }

    public static byte[] HashBytes(this string content) => HashBytes(Encoding.ASCII.GetBytes(content));

    public static string ToHex(this byte[]? data, string defaultMessage = "") => (data is null) ? defaultMessage : string.Concat(data.Select(t => t.ToString("x2")));

    public static byte[] Ripemd160(this byte[] data)
    {
        RipeMD160Digest digest = new();
        var result = new byte[digest.GetDigestSize()];
        digest.BlockUpdate(data, 0, data.Length);
        digest.DoFinal(result, 0);
        return result;
    }

    public static byte[] CheckIfIsNull(byte[]? data)
    {
        if (data != null)
        {
            return data;
        }

        return BitConverter.GetBytes(0x0000000000000000000000000000000000000000000000000000000000000000);
    }

    public static byte[] StringToBytes(this string text)
    {
        return Encoding.ASCII.GetBytes(text);
    }

    public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
    }
}