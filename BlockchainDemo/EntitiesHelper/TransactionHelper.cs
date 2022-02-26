using BlockchainDemo.Entities;
using BlockchainDemo.Helpers;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace BlockchainDemo.EntitiesHelper;

public static class TransactionHelper
{
    public enum RawTransactionType { Global, Signature };
    public static Transaction CreateTransaction(User? user, string toAddress, double amount)
    {
        if (user is not null)
        {
            byte[] fromPrivateKey = user.PrivateKey;
            byte[] fromPublicKey = user.PublicKey;
            string fromAddress = user.Address;

            var previousTransactions = BlockChain.GetOutputTransactionFromAddress(fromAddress);
            double amountTemp = 0;
            List<TransactionInput> trInput = new();
            foreach (var tr in previousTransactions.UtxoCollection)
            {
                if (amountTemp >= amount) break;
                amountTemp += tr.Amount;
                trInput.Add(new TransactionInput(tr.Position, tr.TxId));
            }

            if (amountTemp < amount) throw new ArgumentOutOfRangeException(nameof(amount));

            List<TransactionOutput> outputList = new();
            outputList.Add(new TransactionOutput(amount, toAddress));
            if (amountTemp > amount)
            {
                outputList.Add(new TransactionOutput(amountTemp - amount, fromAddress));
            }

            var trFinal = new Transaction(trInput.ToArray(), outputList.ToArray(), amount);
            trFinal.PublicKey = fromPublicKey;
            trFinal.Signature = trFinal.Sign(fromPrivateKey);
            trFinal.Txid = trFinal.RawTransaction(RawTransactionType.Global).HashBytes();
            return trFinal;
        }
        else
        {
            // Coinbase
            var outputList = new[] {
                    new TransactionOutput(amount, toAddress)
                    };

            var tr = new Transaction(null, outputList, amount, BlockChain.Heigth);
            tr.Txid = tr.RawTransaction(RawTransactionType.Global).HashBytes();
            return tr;
        }
    }

    public static Transaction CreateTransaction(string to, double value)
    {
        // used only for coinbase
        return CreateTransaction(null, to, value);
    }

    private static byte[] Sign(this Transaction transaction, byte[] privateKey)
    {
        var curve = SecNamedCurves.GetByName("secp256k1");
        var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

        var keyParameters = new ECPrivateKeyParameters(new BigInteger(privateKey), domain);

        ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(true, keyParameters);
        var rawTransaction = transaction.RawTransaction(RawTransactionType.Signature);
        signer.BlockUpdate(rawTransaction, 0, rawTransaction.Length);
        return signer.GenerateSignature();
    }

    public static bool VerifySignature(this Transaction transaction, byte[] publicKeyBytes, byte[] signature)
    {
        var curve = SecNamedCurves.GetByName("secp256k1");
        var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

        var q = curve.Curve.DecodePoint(publicKeyBytes);

        var keyParameters = new ECPublicKeyParameters(q, domain);

        ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(false, keyParameters);
        var rawTransaction = transaction.RawTransaction(RawTransactionType.Signature);
        signer.BlockUpdate(rawTransaction, 0, rawTransaction.Length);
        return signer.VerifySignature(signature);
    }

    public static byte[] RawTransaction(this Transaction transaction, RawTransactionType rawTransactionType)
    {
        using MemoryStream ms = new();
        ms.Write(BitConverter.GetBytes(transaction.Value));
        ms.Write(BitConverter.GetBytes(transaction.BlockHeight));
        if (transaction.Input is null || transaction.Input.Length == 0)
        {
            ms.Write(BitConverter.GetBytes(0));
        }
        else
        {
            ms.Write(BitConverter.GetBytes(1));
            for (int i = 0; i < transaction.Input.Length; i++)
            {
                var input = transaction.Input[i];
                ms.Write(input.TransactionRef);
            }

            ms.Write(BitConverter.GetBytes(1));
        }

        if (transaction.Output == null || transaction.Output.Length == 0)
        {
            ms.Write(BitConverter.GetBytes(0));
        }
        else
        {
            ms.Write(BitConverter.GetBytes(1));
            for (int i = 0; i < transaction.Output.Length; i++)
            {
                var output = transaction.Output[i];
                ms.Write(output.RecipientAddress.StringToBytes());
                ms.Write(BitConverter.GetBytes(output.Amount));
            }

            ms.Write(BitConverter.GetBytes(1));
        }

        if (transaction.PublicKey is null)
        {
            ms.Write(BitConverter.GetBytes(0));
        }
        else
        {
            ms.Write(BitConverter.GetBytes(1));
            ms.Write(transaction.PublicKey);
        }

        if (rawTransactionType == RawTransactionType.Global)
        {
            ms.Write(transaction.Signature);
        }

        return ms.ToArray();
    }
}
