using BlockchainDemo;
using BlockchainDemo.Entities;
using BlockchainDemo.EntitiesHelper;
using BlockchainDemo.Helpers;

const int USERS = 4;
const int MAXCOINBASE = 100;
const string DIFFICULTY = "0000";
const string BITS = "0x1effffff";

List<User> users = new(USERS);
ConsoleHelper.PrintTitle("Create users...");

for (int i = 0; i < USERS; i++)
{
    var user = UserHelper.CreateNewUser(MAXCOINBASE, BITS);
    users.Add(user);

    ConsoleHelper.PrintUserInfo(user);
}

// Create genesis block. 100 to user 1
{
    var block = users[0].MinerInstance?.CreateBlock(BlockChain.Heigth, DIFFICULTY, BlockChain.LastHash, null);
    if (block is not null)
    {
        try
        {
            BlockChain.VerifyAndInsert(block, MAXCOINBASE);
            ConsoleHelper.ShowInfoBlock(block, "Create genesis block. 100 to user 1");
            ConsoleHelper.ShowAmountForUsers(users);
        }
        catch (BlockChainVerifyException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    ConsoleHelper.WaitEnter();
}

// Add block with transaction signed: 15 da user 0 to user 1
{
    var block = users[0].MinerInstance?.CreateBlock(BlockChain.Heigth, DIFFICULTY, BlockChain.LastHash, new[]
    {
        TransactionHelper.CreateTransaction(users[0], users[1].Address, 15),
    });

    if (block is not null)
    {
        try
        {
            BlockChain.VerifyAndInsert(block, MAXCOINBASE);
            ConsoleHelper.ShowInfoBlock(block, "Add block with transaction signed: 15 da user 0 to user 1");
            ConsoleHelper.ShowAmountForUsers(users);
        }
        catch (BlockChainVerifyException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    ConsoleHelper.WaitEnter();
}

// Add block with transaction signed 2: 5 da user 1 a user 2
{
    var block = users[0].MinerInstance?.CreateBlock(BlockChain.Heigth, DIFFICULTY, BlockChain.LastHash, new[]
    {
        TransactionHelper.CreateTransaction(users[1], users[2].Address, 5),
    });

    if (block is not null)
    {
        try
        {
            BlockChain.VerifyAndInsert(block, MAXCOINBASE);
            ConsoleHelper.ShowInfoBlock(block, "Add block with transaction signed 2: 5 da user 1 a user 2");
            ConsoleHelper.ShowAmountForUsers(users);
        }
        catch (BlockChainVerifyException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    ConsoleHelper.WaitEnter();
}

// Add block with transaction signed 2: 5 da user 1 a user 2
{
    var block = users[0].MinerInstance?.CreateBlock(BlockChain.Heigth, DIFFICULTY, BlockChain.LastHash, new[]
    {
        TransactionHelper.CreateTransaction(users[1], users[2].Address, 5),
    });

    if (block is not null)
    {
        try
        {
            BlockChain.VerifyAndInsert(block, MAXCOINBASE);
            ConsoleHelper.ShowInfoBlock(block, "Add block with transaction signed 2: 5 da user 1 a user 2");
            ConsoleHelper.ShowAmountForUsers(users);
        }
        catch (BlockChainVerifyException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    ConsoleHelper.WaitEnter();
}

// Add block with transaction signed 3
{
    var block = users[0].MinerInstance?.CreateBlock(BlockChain.Heigth, DIFFICULTY, BlockChain.LastHash, new[]
    {
        TransactionHelper.CreateTransaction(users[0], users[2].Address, 15),
        TransactionHelper.CreateTransaction(users[1], users[3].Address, 5),
    });

    if (block is not null)
    {
        try
        {
            BlockChain.VerifyAndInsert(block, MAXCOINBASE);
            ConsoleHelper.ShowInfoBlock(block, "Add block with transaction signed 3");
            ConsoleHelper.ShowAmountForUsers(users);
        }
        catch (BlockChainVerifyException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    ConsoleHelper.WaitEnter();
}

// Add block with transaction signed 4 mined from user 2
{
    var block = users[1].MinerInstance?.CreateBlock(BlockChain.Heigth, DIFFICULTY, BlockChain.LastHash, new[]
    {
        TransactionHelper.CreateTransaction(users[2], users[3].Address, 25),
    });

    if (block is not null)
    {
        try
        {
            BlockChain.VerifyAndInsert(block, MAXCOINBASE);
            ConsoleHelper.ShowInfoBlock(block, "Add block with transaction signed 4 (mined from user 2)");
            ConsoleHelper.ShowAmountForUsers(users);
        }
        catch (BlockChainVerifyException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    ConsoleHelper.WaitEnter();
}
