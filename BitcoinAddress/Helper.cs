using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;

internal static class Helper
{
    internal static byte[] StringToByteArray(this string hex) => Enumerable.Range(0, hex.Length)
                         .Where(t => t % 2 == 0)
                         .Select(t => Convert.ToByte(hex.Substring(t, 2), 16))
                         .ToArray();

    internal static string ToHex(this byte[] data) => string.Concat(data.Select(t => t.ToString("x2")));

    internal static byte[] HashBytes(this byte[] content)
    {
        using var sha256Hash = SHA256.Create();
        return sha256Hash.ComputeHash(content);

    }

    internal static byte[] Ripemd160(this byte[] data)
    {
        var digest = new RipeMD160Digest();
        var result = new byte[digest.GetDigestSize()];
        digest.BlockUpdate(data, 0, data.Length);
        digest.DoFinal(result, 0);
        return result;
    }
}
