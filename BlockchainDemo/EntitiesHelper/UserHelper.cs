using BlockchainDemo.Entities;
using BlockchainDemo.Helpers;

namespace BlockchainDemo.EntitiesHelper;

public class UserHelper
{
    public static User CreateNewUser(int maxCoinBase, string bits)
    {
        var (privateKey, publicKey) = CryptografyHelper.GetPrivateAndPublicKey();
        var address = CryptografyHelper.GetAddress(publicKey);

        User user = new(address, privateKey, publicKey);
        user.MinerInstance = new Miner(user, maxCoinBase, bits);
        return user;
    }
}