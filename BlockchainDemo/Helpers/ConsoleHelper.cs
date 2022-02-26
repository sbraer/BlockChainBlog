using BlockchainDemo.Entities;
using System.Diagnostics;

namespace BlockchainDemo.Helpers;

public static class ConsoleHelper
{
    public enum TextAlign { Left, Right };
    public enum CharSeparator { Dash, Equal };
    private const int LENGTH = 90;
    public static void PrintTitle(string title, CharSeparator charSeparator = CharSeparator.Equal)
    {
        if (charSeparator == CharSeparator.Equal)
        {
            PrintSeparatorEqual();
        }
        else
        {
            PrintSeparatorDash();
        }
        int leftMargin = title.Length >= LENGTH ? 0 : (LENGTH - title.Length) / 2;
        Console.WriteLine($"{new string(' ', leftMargin)}{title}");
        if (charSeparator == CharSeparator.Equal)
        {
            PrintSeparatorEqual();
        }
        else
        {
            PrintSeparatorDash();
        }
    }

    public static void PrintUserInfo(User user)
    {
        Console.WriteLine($"{FixedWidthString("Address", 15)} {user.Address}");
        Console.WriteLine($"{FixedWidthString("Private Key", 15)} {user.PrivateKey.ToHex()}");
        Console.WriteLine($"{FixedWidthString("Public Key", 15)} {FixedWidthString(user.PublicKey.ToHex(), 74)}");
        Console.WriteLine();
    }

    public static void PrintSeparatorEqual()
    {
        Console.WriteLine(Separator('='));
    }

    public static void PrintSeparatorDash()
    {
        Console.WriteLine(Separator('-'));
    }

    private static string Separator(char c)
    {
        return new string(c, LENGTH);
    }

    private static string FixedWidthString(string? text, int maxChars, TextAlign textAlign = TextAlign.Left)
    {
        if (text is null)
        {
            text = string.Empty;
        }

        if (text.Length > maxChars)
        {
            string points = maxChars % 2 == 0 ? ".." : ".";
            int half = maxChars / 2 - (maxChars % 2 == 0 ? 1 : 0);
            return $"{text[..half]}{points}{text[^half..]}";
        }
        else if (text.Length < maxChars)
        {
            string returnValue = string.Empty;
            if (textAlign == TextAlign.Right)
            {
                returnValue += new string(' ', maxChars - text.Length);
            }

            returnValue += text;

            if (textAlign == TextAlign.Left)
            {
                returnValue += new string(' ', maxChars - text.Length);
            }

            return returnValue;
        }
        else
        {
            return text;
        }
    }

    public static void ShowInfoBlock(Block block, string title = "")
    {
        const int rightColumn = 26;
        PrintTitle($"Block {block.Height} - {title}");
        Console.WriteLine($"{FixedWidthString("Hash", rightColumn)}{block.HashBlock.ToHex()}");
        Console.WriteLine($"{FixedWidthString("Time", rightColumn)}{block.BlockHeader.Time.UnixTimeStampToDateTime():yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"{FixedWidthString("Nonce", rightColumn)}{block.BlockHeader.Nonce}");
        Console.WriteLine($"{FixedWidthString("Bits", rightColumn)}{block.BlockHeader.Bits}");
        Console.WriteLine($"{FixedWidthString("Hash previous block", rightColumn)}{block.BlockHeader.HashPrevBlock?.ToHex()}");
        Console.WriteLine($"{FixedWidthString("Hash Merkle root", rightColumn)}{block.BlockHeader.HashMerkleRoot.ToHex()}");
        Console.WriteLine($"{FixedWidthString("Time to mining", rightColumn)}{block.TimeToMine}");

        PrintTitle("Transactions", CharSeparator.Dash);
        var trColl = block.Transactions;
        Console.WriteLine($"{FixedWidthString("Numero", rightColumn)}{block.TransactionCounter}");
        for (int i = 0; i < trColl.Length; i++)
        {
            var tr = trColl[i];
            Console.WriteLine($"{FixedWidthString($"{i + 1} - Txid", rightColumn)}{tr.Txid.ToHex()}");
            Console.WriteLine($"{FixedWidthString($"{i + 1} - Public Key", rightColumn)}{FixedWidthString(tr.PublicKey?.ToHex(), 64)}");
            Console.WriteLine($"{FixedWidthString($"{i + 1} - Signature", rightColumn)}{FixedWidthString(tr.Signature?.ToHex(), 64)}");

            Console.WriteLine("Input");
            if (tr.Input is null || tr.Input.Length == 0)
            {
                Console.WriteLine("-");
            }
            else
            {
                for (int x = 0; x < tr.Input.Length; x++)
                {
                    var input = tr.Input[x];
                    Console.WriteLine($"{FixedWidthString("Position", rightColumn)}{input.Position}");
                    Console.WriteLine($"{FixedWidthString("Txid", rightColumn)}{FixedWidthString(input.TransactionRef.ToHex(), 64)}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Output");
            if (tr.Output is null || tr.Output.Length == 0)
            {
                Console.WriteLine("-");
            }
            else
            {
                for (int x = 0; x < tr.Output.Length; x++)
                {
                    var output = tr.Output[x];
                    Console.WriteLine($"{FixedWidthString("Amount", rightColumn)}{output.Amount}");
                    Console.WriteLine($"{FixedWidthString("Recipient Address", rightColumn)}{output.RecipientAddress}");
                }
            }

            PrintSeparatorDash();
        }

        Console.WriteLine();
    }

    public static void ShowAmountForUsers(List<User> users)
    {
        Console.WriteLine("Show amount for users");
        {
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                Console.WriteLine($"{i + 1} {user.Address} = {BlockChain.GetOutputTransactionFromAddress(user.Address).TotalAmount}");
            }
        }
    }

    [Conditional("DEBUG")]
    public static void WaitEnter()
    {
        Console.WriteLine();
        Console.Write("Press 'enter' to continue... ");
        Console.ReadLine();
        Console.WriteLine();
    }
}