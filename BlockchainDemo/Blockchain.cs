using BlockchainDemo.Entities;
using BlockchainDemo.EntitiesHelper;
using BlockchainDemo.Helpers;

namespace BlockchainDemo;

public class BlockChainVerifyException : Exception
{
    public BlockChainVerifyException(string message) : base(message) { }
}

public static class BlockChain
{
    private static readonly List<Block> _blocks = new();

    public static int Heigth => _blocks.Count;

    public static byte[]? LastHash => _blocks.Count == 0 ? null : _blocks.Last().HashBlock;

    public static void VerifyAndInsert(Block block, int MAXCOINBASE)
    {
        // It's not implemented multiple transaction from the same input
        if (block.Transactions.GroupBy(t => t.PublicKey).Where(t => t.Count() > 1).Any())
        {
            throw new BlockChainVerifyException("Only one transaction for block!");
        }

        // Check signature in transaction
        foreach (var tr in block.Transactions)
        {
            // Check if is coinbase
            if (tr.PublicKey is null && tr.Signature is null)
            {
                if (tr.Input is not null
                    || tr.Output.Length != 1
                    || tr.Output[0].Amount != MAXCOINBASE
                    || tr.Output[0].RecipientAddress != block.MinerAddress)
                {
                    throw new BlockChainVerifyException($"Coinbase error: {tr.Txid.ToHex("-")}");
                }
            }
            else if (!(tr.PublicKey is not null && tr.Signature is not null))
            {
                throw new BlockChainVerifyException("PublicKey and Signature are inconsistent");
            }
            else
            {
                if (tr.VerifySignature(tr.PublicKey, tr.Signature) == false)
                {
                    throw new BlockChainVerifyException($"Signature is wrong: {tr.Txid.ToHex("-")}");
                }

                // Check is amount is available
                var amount = tr.Output.Sum(t => t.Amount);
                double tempAmount = 0;
                for (int v = 0; tr.Input is not null && v < tr.Input.Length; v++)
                {
                    var input = tr.Input[v];
                    var available = TransactionIsAvailable(input.TransactionRef, input.Position);
                    if (available.IsAvailable == false)
                    {
                        Console.WriteLine($">>>> IS not available {input.TransactionRef.ToHex()}");
                        throw new BlockChainVerifyException($"Transaction is not available: {tr.Txid.ToHex()}");
                    }

                    tempAmount += available.Amount;
                }

                if (amount > tempAmount)
                {
                    throw new BlockChainVerifyException($"Transaction amount is not available: {tr.Txid.ToHex()}");
                }
            }
        }

        _blocks.Add(block);
    }

    public static (double TotalAmount, Utxo[] UtxoCollection) GetOutputTransactionFromAddress(string fromAddress)
    {
        double amount = 0;
        var tempColl = new List<Utxo>();
        for (int i = 0; i < _blocks.Count; i++)
        {
            var block = _blocks[i];
            for (int y = 0; y < block.TransactionCounter; y++)
            {
                var tr = block.Transactions[y];
                if (tr.Txid is null) continue;
                for (int x = 0; x < tr.Output.Length; x++)
                {
                    var output = tr.Output[x];
                    if (output.RecipientAddress == fromAddress)
                    {
                        tempColl.Add(new Utxo(x, output.Amount, tr.Txid, false));
                    }
                }

                for (int x = 0; tr.Input is not null && x < tr.Input.Length; x++)
                {
                    var input = tr.Input[x];
                    for (int v = 0; v < tempColl.Count; v++)
                    {
                        var item = tempColl[v];
                        if (item.Position == input.Position && item.TxId.ToHex() == input.TransactionRef.ToHex())
                        {
                            item.Used = true;
                            amount -= item.Amount;
                        }
                    }
                }
            }
        }

        amount += tempColl.Where(t => !t.Used).Sum(t => t.Amount);
        return (amount, tempColl.Where(t => !t.Used).OrderBy(t => t.Amount).ToArray());
    }

    private static (bool IsAvailable, double Amount) TransactionIsAvailable(byte[] txid, int position)
    {
        bool avaiable = true;
        double amount = 0;

        for (int i = 0; i < _blocks.Count; i++)
        {
            var block = _blocks[i];
            for (int y = 0; y < block.TransactionCounter; y++)
            {
                var tr = block.Transactions[y];
                for (int v = 0; tr.Input is not null && v < tr.Input.Length; v++)
                {
                    var input = tr.Input[v];
                    if (input.TransactionRef.ToHex() == txid.ToHex() && input.Position == position)
                    {
                        return (false, 0);
                    }
                }

                if (txid.ToHex() == tr.Txid.ToHex())
                {
                    for (int v = 0; v < tr.Output.Length; v++)
                    {
                        var output = tr.Output[v];
                        amount += output.Amount;
                    }
                }
            }
        }

        return (avaiable, amount);
    }
}