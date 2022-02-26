namespace BlockchainDemo.Entities;

public class Block
{
    public Block(string minerAddress, long height, BlockHeader blockHeader, Transaction[] transactions)
    {
        MinerAddress = minerAddress;
        Height = height;
        BlockHeader = blockHeader;
        Transactions = transactions;
        TransactionCounter = transactions.Length;
        TimeToMine = 0;
    }

    public string MinerAddress { get; init; }
    public long Height { get; init; }
    public byte[]? HashBlock { get; set; } = null;
    public BlockHeader BlockHeader { get; set; }
    public int TransactionCounter { get; init; }
    public Transaction[] Transactions { get; init; }
    public long TimeToMine { get; set; }
}

public class BlockHeader
{
    public BlockHeader(string bits, long time, uint nonce, byte[]? previousBlock)
    {
        Bits = bits;
        Time = time;
        Nonce = nonce;
        HashPrevBlock = previousBlock;
    }

    public byte[]? HashPrevBlock { get; init; }
    public byte[]? HashMerkleRoot { get; set; }
    public long Time { get; set; }
    public string Bits { get; init; }
    public uint Nonce { get; set; }
}
