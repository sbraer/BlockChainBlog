namespace BlockchainDemo.Entities;

public struct Utxo
{
    public Utxo(int position, double amount, byte[] txid, bool used)
    {
        Position = position;
        Amount = amount;
        TxId = txid;
        Used = used;
    }

    public int Position { get; init; }
    public double Amount { get; init; }
    public byte[] TxId { get; init; }
    public bool Used { get; set; } = false;
}
