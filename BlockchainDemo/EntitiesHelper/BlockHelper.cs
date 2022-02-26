using BlockchainDemo.Entities;
using BlockchainDemo.Helpers;
using System.Diagnostics;

namespace BlockchainDemo.EntitiesHelper;

public static class BlockHelper
{
    public static void SetMerkleRoot(this Block block)
    {
        var coll = block.Transactions.Where(t => t.Txid is not null).Select(t => t.Txid!).ToList();
        while (coll.Count > 1)
        {
            List<byte[]> buffer = new();
            for (int i = 0; i < coll.Count; i += 2)
            {
                var item1 = coll[i];
                var item2 = coll[i + (i + 1 == coll.Count ? 0 : 1)];

                using var ms = new MemoryStream();
                ms.Write(item1.HashBytes());
                ms.Write(item2.HashBytes());
                buffer.Add(ms.ToArray().HashBytes());
            }

            coll = buffer;
        }

        block.BlockHeader.HashMerkleRoot = coll[0];
    }

    public static byte[] GetHashBlock(this BlockHeader blockHeader)
    {
        using MemoryStream ms = new();
        ms.Write(BitConverter.GetBytes(0x00000001)); // version
        ms.Write(Helper.CheckIfIsNull(blockHeader.HashPrevBlock));
        ms.Write(blockHeader.HashMerkleRoot);
        ms.Write(BitConverter.GetBytes(blockHeader.Time));
        ms.Write(blockHeader.Bits.StringToBytes());
        ms.Write(BitConverter.GetBytes(blockHeader.Nonce));
        return ms.ToArray().HashBytes();
    }

    public static void MineHash(this Block block, string difficulty)
    {
        bool found = false;
        var blockHeader = block.BlockHeader;

        Stopwatch sw = new();
        sw.Start();

        while (true)
        {
            for (uint i = 0; i <= 4_294_967_295; i++)
            {
                // Set hash for the block
                blockHeader.Nonce = i;
                var hash = blockHeader.GetHashBlock();
                var hashString = hash.ToHex();
                if (hashString.StartsWith(difficulty))
                {
                    found = true;
                    block.HashBlock = hash;
                    break;
                }
            }

            if (found) break;
            blockHeader.Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        sw.Stop();
        block.TimeToMine = sw.ElapsedMilliseconds;
    }
}