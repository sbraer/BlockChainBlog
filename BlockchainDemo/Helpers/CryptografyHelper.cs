using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SimpleBase;

namespace BlockchainDemo.Helpers;

public class CryptografyHelper
{
    public static (byte[] privateKey, byte[] publicKey) GetPrivateAndPublicKey()
    {
        var curve = ECNamedCurveTable.GetByName("secp256k1");
        var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());

        SecureRandom secureRandom = new();
        ECKeyGenerationParameters keyParams = new(domainParams, secureRandom);

        ECKeyPairGenerator generator = new("ECDSA");
        generator.Init(keyParams);
        var keyPair = generator.GenerateKeyPair();

        var privateKeyPar = (ECPrivateKeyParameters)keyPair.Private;
        var publicKeyPar = (ECPublicKeyParameters)keyPair.Public;

        return (privateKeyPar.D.ToByteArray(), publicKeyPar.Q.GetEncoded());
    }

    public static string GetAddress(byte[] publicKey)
    {
        var hashPublicKey = publicKey.HashBytes();
        var ripemd160 = hashPublicKey.Ripemd160();

        var extendedRipemd160 = new byte[ripemd160.Length + 1];
        extendedRipemd160[0] = 0x00;
        for (int i = 0; i < ripemd160.Length; i++)
        {
            extendedRipemd160[i + 1] = ripemd160[i];
        }

        var hashExtended = extendedRipemd160.HashBytes();
        var doubleHashExtended = hashExtended.HashBytes();

        var extendedChecksum = new byte[extendedRipemd160.Length + 4];
        for (int i = 0; i < extendedRipemd160.Length; i++)
        {
            extendedChecksum[i] = extendedRipemd160[i];
        }

        for (int i = 0; i < 4; i++)
        {
            extendedChecksum[extendedRipemd160.Length + i] = doubleHashExtended[i];
        }

        return Base58.Bitcoin.Encode(extendedChecksum);
    }
}