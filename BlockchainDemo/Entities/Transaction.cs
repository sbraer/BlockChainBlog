namespace BlockchainDemo.Entities;

public class Transaction
{
    public Transaction(TransactionInput[]? input, TransactionOutput[] output, double value, int blockHeight = 0)
    {
        Input = input;
        Output = output;
        Value = value;
        BlockHeight = blockHeight;
    }
    public double Value { get; init; }
    public TransactionInput[]? Input { get; set; }
    public TransactionOutput[] Output { get; init; }
    public byte[]? Txid { get; set; }
    public byte[]? Signature { get; set; }
    public byte[]? PublicKey { get; set; }
    public int BlockHeight { get; set; }

}

public class TransactionInput
{
    public TransactionInput(int position, byte[] transactionRef)
    {
        Position = position;
        TransactionRef = transactionRef;
    }
    public byte[] TransactionRef { get; init; }
    public int Position { get; init; }
}

public class TransactionOutput
{
    public TransactionOutput(double amount, string recipientAddress)
    {
        Amount = amount;
        RecipientAddress = recipientAddress;
    }
    public double Amount { get; set; }
    public string RecipientAddress { get; init; }
}
