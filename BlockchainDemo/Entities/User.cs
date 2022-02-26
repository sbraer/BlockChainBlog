namespace BlockchainDemo.Entities;

public class User
{
    public User(string address, byte[] privateKey, byte[] publicKey)
    {
        Address = address;
        PrivateKey = privateKey;
        PublicKey = publicKey;
    }

    public string Address { get; init; }
    public byte[] PrivateKey { get; init; }
    public byte[] PublicKey { get; init; }
    public Miner? MinerInstance { get; set; } = null;
}
