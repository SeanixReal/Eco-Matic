using System;
using System.Linq;
using System.Threading;
using System.IO;

class Program
{


    public static void Main(string[] args)
    {
        // initialize ecoMatic instance with the fp for inventory and eventLog
        try
        {
            EcoMatic ecoMatic = new EcoMatic("inventory.csv", "eventLog.txt", "data");

            Write.DelayLine("Eco-Matic is still in early developent");

            Write.DelayLoad("Loading");

            MainMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Oopsie... Something went wrong: " + ex.Message);
            return;
        }
    }

    public static void MainMenu()
    {
        throw new Exception("Test");
    }

}

// helper class so i have fewer lines of code when using thread sleep
class Write
{
    public static void DelayLine(string message, int delay = 1000)
    {
        Console.WriteLine(message);
        Thread.Sleep(delay);
    }

    public static void Delay(string message, int delay = 1000)
    {
        Console.Write(message);
        Thread.Sleep(delay);
    }

    public static void DelayLoad(string message, int delay = 1000)
    {
        Write.Delay(message);
        for (int i = 0; i < 3; i++)
        {
            Write.Delay(".", 500);
        }
        Console.Write("\n");
    }
}

class EcoMatic
{
    //min 6
    public const int MaxItems = 6;
    public const int MaxStocks = 10;

    //files and directories
    private string _inventoryName;
    private string _eventLogName;
    private string _dataDirectory;

    //initialize inventory of type vendingitem to contain its different kinds (snacks, drinks, misc)
    private VendingItem[] _inventory = new VendingItem[MaxItems];
    private int _itemCount = 0;



    public EcoMatic(string inventoryName, string eventLogName, string directoryFP)
    {
        // initialize inventory and event log and directory
        _inventoryName = inventoryName;
        _eventLogName = eventLogName;
        _dataDirectory = directoryFP;

        // runs a check on directory data
        // runs a check on both files before running program to prevent errors
        CheckDirectory();
        CheckFiles();
    }

    private void CheckFiles()
    {
        CheckInventoryFile();
        CheckEventLogFile();
    }

    public void CheckDirectory()
    {
        if (!Directory.Exists(_dataDirectory))
        {
            Write.DelayLine($"Directory {_dataDirectory} doesn't exist");
            Write.DelayLoad($"Creating new {_dataDirectory} directory");
            Directory.CreateDirectory(_dataDirectory);
        }
    }

    // check for inventory validity  
    private void CheckInventoryFile()
    {
        string filePath = $"{_dataDirectory}/{_inventoryName}";

        if (!File.Exists(filePath))
        {
            Write.DelayLine($"{_inventoryName} file doesn't exist.");
            Write.DelayLoad($"Creating new {_inventoryName} file");
            CreateDefaultInventoryFile();
            return;
        }

        // check header if correct
        string[] lines = File.ReadAllLines(filePath);
        int length = lines.Length;
        if (lines[0] != "Type,Name,Price,Stock,Calories/Volume" || length <= 1)
        {
            Write.DelayLine("Something is wrong with the inventory.csv");
            Write.DelayLoad($"Fixing {_inventoryName}");
            CreateDefaultInventoryFile();
            return;
        }

        //check for potential formatting errors to prevent future errors
        for (int i = 1; i < length; i++)
        {
            // catch for parse error or formatting error and create default file if error
            try
            {
                string[] parts = lines[i].Split(',');

                string type = parts[0].Trim().ToLower();
                if (type != "drink" && type != "snack" && type != "misc") 
                    throw new FormatException();

                string name = parts[1].Trim();
                decimal price = decimal.Parse(parts[2]);

                int stock = int.Parse(parts[3]);
                if (stock > MaxStocks || stock < 0) throw new FormatException();

                if (type != "misc")
                {
                    double value = double.Parse(parts[4]);
                }

            }
            catch (FormatException)
            {
                Write.DelayLine($"Abnormalities found on line {i + 1}");
                Write.DelayLoad($"Creating new {_inventoryName} file");
                CreateDefaultInventoryFile();
                return;
            }
        }
        
    }

    // defualt inventory
    private void CreateDefaultInventoryFile()
    {
        string filePath = $"{_dataDirectory}/{_inventoryName}";
        using (StreamWriter w = new StreamWriter(filePath))
        {
            w.WriteLine("Type,Name,Price,Stock,Calories/Volume");
            w.WriteLine("Snack,Mr Chips,30.50,10,160");
            w.WriteLine("Snack,Nova,40,10,180");
            w.WriteLine("Drink,Coca Cola,30.50,10,500");
            w.WriteLine("Drink,Pepsi,30,10,500");
            w.WriteLine("Misc,Bandaid Box,20,10,");
            w.WriteLine("Misc,Eco Bag,30.75,10,");
        }
    }

    // load inventory file into local inventory
    private void LoadInventory()
    {
        string filePath = $"{_dataDirectory}/{_inventoryName}";
        string[] lines = File.ReadAllLines(filePath);
        int length = lines.Length;
    }

    private void CheckEventLogFile()
    {
        
    }
}

interface IHasVolume
{
    public double Volume { get; set; }
}

interface IHasCalories
{
    public double Calories { get; set; }
}

abstract class VendingItem
{
    // private set so it can only be modified wihin the class
    public string ItemName { get; private set; }
    public decimal ItemPrice { get; private set; }
    public int ItemStock { get; private set; }

    protected VendingItem(string itemName, decimal itemPrice, int itemStock)
    {
        ItemName = itemName;
        ItemPrice = ItemPrice;
        ItemStock = itemStock;
    }

    public abstract string GetDispenseMessage();
    public abstract string GetExamineMessage();
}

class SnackItem : VendingItem, IHasCalories
{
    public double Calories { get; set; }

    public SnackItem(string itemName, decimal itemPrice, int itemStock, double calories) : base(itemName, itemPrice, itemStock)
    {
        Calories = calories;
    }

    public override string GetDispenseMessage()
    {
        return $"Enjoy your tasty {ItemName}!";
    }

    public override string GetExamineMessage()
    {
        return $"A tasty {ItemName} with {Calories}kcal worth of deliciousness.";
    }
}

class DrinkItem : VendingItem, IHasVolume
{
    public double Volume { get; set; }

    public DrinkItem(string itemName, decimal itemPrice, int itemStock, double volume) : base(itemName, itemPrice, itemStock)
    {
        Volume = volume;
    }

    public override string GetDispenseMessage()
    {
        return $"Enjoy your refreshing {ItemName}!";
    }

    public override string GetExamineMessage()
    {
        return $"A cold {ItemName} with {Volume}ml of sweetness";
    }
}

class MiscItem : VendingItem
{
    public MiscItem(string itemName, decimal itemPrice, int itemStock) : base(itemName, itemPrice, itemStock) { }

    public override string GetDispenseMessage()
    {
        return $"This {ItemName} is going to be useful!";
    }

    public override string GetExamineMessage()
    {
        return $"A useful {ItemName}";
    }
}



