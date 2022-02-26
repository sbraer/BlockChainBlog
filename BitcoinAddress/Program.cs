using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using SimpleBase;

var privateKey = "18e14a7b6a307f426a94f8114701e7c8e774e7f9a47e2c2035db29a206321725".StringToByteArray();
Console.WriteLine($"PrivateKey: {privateKey.ToHex()}");

var curve = SecNamedCurves.GetByName("secp256k1");
var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

var d = new BigInteger(privateKey);
ECPoint ec = domain.G.Multiply(d).Normalize();
Console.WriteLine($"    X cord: {ec.XCoord.ToBigInteger().ToByteArrayUnsigned().ToHex()}");

var publicKeyXInt = ec.XCoord.ToBigInteger();
var publicKeyYInt = ec.YCoord.ToBigInteger();
var publicKeyTemp = publicKeyXInt.ToByteArrayUnsigned();
var publicKey = new byte[publicKeyTemp.Length + 1];
publicKey[0] = (byte)(publicKeyYInt.LongValue % 2 == 0 ? 0x02 : 0x03);
for (int i = 0; i < publicKeyTemp.Length; i++)
{
    publicKey[i + 1] = publicKeyTemp[i];
}

var hashPublicKey = publicKey.HashBytes();
Console.WriteLine($"Hashpubkey: {hashPublicKey.ToHex()}");

var ripemd160 = hashPublicKey.Ripemd160();
Console.WriteLine($"Ripemd-160: {ripemd160.ToHex()}");

var extendedRipemd160 = new byte[ripemd160.Length + 1];
extendedRipemd160[0] = 0x00;
for (int i = 0; i < ripemd160.Length; i++)
{
    extendedRipemd160[i + 1] = ripemd160[i];
}

Console.WriteLine($"Ripemd-ext: {extendedRipemd160.ToHex()}");

var hashExtended = extendedRipemd160.HashBytes();
Console.WriteLine($"Exten hash: {hashExtended.ToHex()}");

var doubleHashExtended = hashExtended.HashBytes();
Console.WriteLine($"Doublehash: {doubleHashExtended.ToHex()}");

var extendedChecksum = new byte[extendedRipemd160.Length + 4];
for (int i = 0; i < extendedRipemd160.Length; i++)
{
    extendedChecksum[i] = extendedRipemd160[i];
}

for (int i = 0; i < 4; i++)
{
    extendedChecksum[extendedRipemd160.Length + i] = doubleHashExtended[i];
}

Console.WriteLine($"   Address: {extendedChecksum.ToHex()}");

string address = Base58.Bitcoin.Encode(extendedChecksum);
Console.WriteLine($"    Base58: {address}");