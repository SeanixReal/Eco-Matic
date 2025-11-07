using System;
using System.Linq;
using System.Threading;
using System.IO;
using Spectre.Console;
using System.Runtime.InteropServices;

class Program
{
    public static void Main(string[] args)
    {
        Console.Clear();
        Console.Title = "Eco-Matic Vending Machine";
        // initialize ecoMatic instance with the fp for inventory and eventLog
        try
        {
            EcoMatic ecoMatic = new EcoMatic("inventory.csv", "eventLog.csv", "data");

            Write.DelayLine("Eco-Matic is still in early developent");

            Write.DelayLoad("Loading");

            MainMenu(ecoMatic);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Oopsie... Something went wrong. " + ex.Message);
            return;
        }
    }

    public static void MainMenu(EcoMatic ecoMatic)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Main Menu");
            Console.WriteLine("1. Customer");
            Console.WriteLine("2. Admin");
            Console.WriteLine("3. Exit");
            Console.Write("\nChoice: ");

            string choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1":
                    CustomerMenu(ecoMatic);
                    break;
                case "2":
                    AdminMenu(ecoMatic);
                    break;
                case "3":
                    Write.DelayLine("Thank you for using Eco-Matic Vending Machine");
                    Write.DelayLine("Have a great and awesome day!");
                    Write.DelayLoad("Exiting");
                    return;
            }
        }

    }

    public static void CustomerMenu(EcoMatic ecoMatic)
    {
        ecoMatic.ShowInventory();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public static void AdminMenu(EcoMatic ecoMatic)
    {
        Write.DelayLoad("Still in development");
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
    private string _inventoryFileName;
    private string _eventLogFileName;
    private string _dataDirectory;

    //initialize inventory of type vendingitem to contain its different kinds (snacks, drinks, misc)
    private VendingItem[] _inventory = new VendingItem[MaxItems];
    private int _itemCount = 0;
    private decimal _currentBalance = 0;

    public EcoMatic(string inventoryName, string eventLogName, string directoryFP)
    {
        // initialize inventory and event log and directory
        _inventoryFileName = inventoryName;
        _eventLogFileName = eventLogName;
        _dataDirectory = directoryFP;

        // runs a check on directory data
        // runs a check on both files before running program to prevent errors
        CheckDirectory();
        CheckFiles();

        //load inventory file into local _inventory 
        LoadInventory();
    }

    // customer methods

    //show inventory in a table using nuget package spectre console
    public void ShowInventory()
    {
        Console.Clear();

        // create format of table
        Table table = new Table();
        table.Border = TableBorder.Rounded;
        table.BorderColor(Color.Green);
        table.Title = new TableTitle("[bold green]╔═══ ECO-MATIC VENDING MACHINE ═══╗[/]");
        table.AddColumn(new TableColumn("[bold yellow]ID[/]").Centered());
        table.AddColumn(new TableColumn("[bold yellow]Item[/]").LeftAligned());
        table.AddColumn(new TableColumn("[bold yellow]Price[/]").Centered());
        table.AddColumn(new TableColumn("[bold yellow]Stock[/]").Centered());

        // set up the data to the table to be printed  after
        for (int i = 0; i < _itemCount; i++)
        {
            VendingItem item = _inventory[i];
            
            string stockIndicator;
            if (item.ItemStock == 0)
            {
                stockIndicator = "[red]NO STOCK[/]";
            }
            else if (item.ItemStock <= 3)
            {
                stockIndicator = "[red]●[/]";
            }
            else if (item.ItemStock <= 6)
            {
                stockIndicator = "[yellow]● ●[/]";
            }
            else
            {
                stockIndicator = "[green]● ● ●[/]";
            }
            
            table.AddRow(
                $"[cyan]{i + 1}[/]", // item num
                $"[bold]{item.ItemName}[/]", // name
                $"[green]₱{item.ItemPrice:F2}[/]", //price
                stockIndicator // stock indicator string 
            );
        }

        AnsiConsole.Write(table);
        
        AnsiConsole.MarkupLine($"\n[bold green]Current Balance: ₱{_currentBalance}[/]");
    }

    //wrapper
    private void CheckFiles()
    {
        CheckInventoryFile();
        CheckEventLogFile();
    }
    
    // checks directory validity
    public void CheckDirectory()
    {
        try
        {
            if (!Directory.Exists(_dataDirectory))
            {
                Write.DelayLine($"Directory {_dataDirectory} doesn't exist");
                Write.DelayLoad($"Creating new {_dataDirectory} directory");
                Directory.CreateDirectory(_dataDirectory);
            }
        }
        catch (IOException ex)
        {
            Write.DelayLine("Error checking directory. " + ex.Message);
        }
        
    }

    // check for inventory validity  
    private void CheckInventoryFile()
    {
        string filePath = Path.Combine(_dataDirectory, _inventoryFileName);

        if (!File.Exists(filePath))
        {
            Write.DelayLine($"{_inventoryFileName} file doesn't exist.");
            Write.DelayLoad($"Creating new {_inventoryFileName} file");
            CreateDefaultInventoryFile();
            return;
        }

        // check header if correct
        string[] lines = File.ReadAllLines(filePath);
        int length = lines.Length;
        if (lines[0] != "Type,Name,Price,Stock,Calories/Volume" || length <= 1)
        {
            Write.DelayLine($"Something is wrong with {_inventoryFileName}");
            Write.DelayLoad($"Fixing {_inventoryFileName}");
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
                Write.DelayLoad($"Creating new {_inventoryFileName} file");
                CreateDefaultInventoryFile();
                return;
            }
        }
    }

    // defualt inventory
    private void CreateDefaultInventoryFile()
    {
        try
        {
            string filePath = Path.Combine(_dataDirectory, _inventoryFileName);
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
        catch (IOException ex)
        {
            Write.DelayLine("Error updating inventory. " + ex.Message);
        }
        
    }

    // load inventory file into local inventory
    private void LoadInventory()
    {
        string filePath = Path.Combine(_dataDirectory, _inventoryFileName);
        string[] lines = File.ReadAllLines(filePath);
        int length = lines.Length;
        // inventory.csv to _inventory
        for (int i = 1; i < length; i++)
        {
            string[] parts = lines[i].Split(',');

            string itemType = parts[0].Trim().ToLower();
            string itemName = parts[1];
            decimal itemPrice = decimal.Parse(parts[2]);
            int itemStock = int.Parse(parts[3]);

            if (itemType == "snack")
            {
                double calories = double.Parse(parts[4]);
                _inventory[_itemCount++] = new SnackItem(itemName, itemPrice, itemStock, calories);
            }
            else if (itemType == "drink")
            {
                double volume = double.Parse(parts[4]);
                _inventory[_itemCount++] = new DrinkItem(itemName, itemPrice, itemStock, volume);
            }
            else if (itemType == "misc")
            {
                _inventory[_itemCount++] = new MiscItem(itemName, itemPrice, itemStock);
            }
            else
            {
                throw new Exception($"Something is wrong with the {_inventoryFileName}. Please rerun program to fix.");
            }
        }
    }

    //update inventory
    private void UpdateInventory()
    {
        string filePath = Path.Combine(_dataDirectory, _inventoryFileName);
        try
        {
            using (StreamWriter w = new StreamWriter(filePath))
            {
                w.WriteLine("Type,Name,Price,Stock,Calories/Volume");

                // _inventory to inventory.csv
                for (int i = 0; i < _itemCount; i++)
                {
                    w.WriteLine(_inventory[i].ToCsvLine());
                }
            }

            Write.DelayLine($"{_inventoryFileName} updated succesfully.");
        }
        catch (IOException ex)
        {
            Write.DelayLine("Error updating inventory. " + ex.Message);
        }
    }

    // log event to be added to eventlog csv
    private void LogEvent(string eventType, string action, string itemName, decimal unitPrice, int quantity, string details)
    {
        string filePath = Path.Combine(_dataDirectory, _eventLogFileName);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logLine = $"{timestamp},{eventType},{action},{itemName},{unitPrice},{quantity},{details}";

        try
        {
            using (StreamWriter w = new StreamWriter(filePath, true))
            {
                w.WriteLine(logLine);
            }
        }
        catch (IOException ex)
        {
            Write.DelayLine("Error logging event. " + ex.Message);
        }
    }

    // checks even log file for errors
    private void CheckEventLogFile()
    {
        string filePath = Path.Combine(_dataDirectory, _eventLogFileName);

        if (!File.Exists(filePath))
        {
            Write.DelayLine($"{_eventLogFileName} file doesn't exist.");
            Write.DelayLoad($"Creating new {_eventLogFileName} file");
            CreateDefaultEventLogFile();
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length == 0 || lines[0] != "Timestamp,EventType,Action,ItemName,UnitPrice,Quantity,Details")
        {
            Write.DelayLine($"Something is wrong with {_eventLogFileName}");
            Write.DelayLoad($"Fixing {_eventLogFileName}");
            CreateDefaultEventLogFile();
            return;
        }
    }

    // make default event log 
    private void CreateDefaultEventLogFile()
    {
        string filePath = Path.Combine(_dataDirectory, _eventLogFileName);
        using (StreamWriter w = new StreamWriter(filePath))
        {
            w.WriteLine("Timestamp,EventType,Action,ItemName,UnitPrice,Quantity,Details");
        }
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
        ItemPrice = itemPrice;
        ItemStock = itemStock;
    }

    public abstract string GetDispenseMessage();
    public abstract string GetExamineMessage();
    public abstract string ToCsvLine();
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

    public override string ToCsvLine()
    {
        return $"Snack,{ItemName},{ItemPrice},{ItemStock},{Calories}";
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

    public override string ToCsvLine()
    {
        return $"Drink,{ItemName},{ItemPrice},{ItemStock},{Volume}";
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

    public override string ToCsvLine()
    {
        return $"Misc,{ItemName},{ItemPrice},{ItemStock},";
    }
}



