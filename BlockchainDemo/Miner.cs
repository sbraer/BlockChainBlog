using BlockchainDemo.Entities;
using BlockchainDemo.EntitiesHelper;

namespace BlockchainDemo;

public class Miner
{
    private readonly User _user;
    private readonly int _maxCoinBase;
    private readonly string _bits;

    public Miner(User user, int maxCoinBase, string bits)
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _maxCoinBase = maxCoinBase;
        _bits = bits;
        if (maxCoinBase < 1)
        {
            throw new ArgumentException("maxCoinBase cannot be less than 1");
        }
    }

    public Block? CreateBlock(int height, string difficulty, byte[]? previousBlock, params Transaction[]? transactions)
    {
        try
        {
            List<Transaction> newTransaction = new();
            if (transactions != null && transactions.Length > 0)
            {
                newTransaction.AddRange(transactions);
            }

            // Add coinbase transaction
            newTransaction.Add(TransactionHelper.CreateTransaction(_user.Address, _maxCoinBase));

            BlockHeader blockHeader = new(_bits, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 0, previousBlock);
            var block = new Block(_user.Address, height, blockHeader, newTransaction.ToArray());

            block.SetMerkleRoot();
            block.MineHash(difficulty);

            return block;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"-- Error: {ex.Message}");
            return null;
        }
    }
}