using System;
using System.Linq;
using System.Threading;
using System.IO;
using Spectre.Console;
using System.Text;

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
            Write.Error("Oopsie... Something went wrong. " + ex.Message);
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
        while(true)
        {
            ecoMatic.ShowInventory();
            Console.WriteLine("\n1. Insert Money");
            Console.WriteLine("2. Buy Item");
            Console.WriteLine("3. Examine Item");
            Console.WriteLine("4. Recycle Item");
            Console.WriteLine("5. Get Change and Return");
            Console.Write("Choice: ");
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    CustomerInsertMoneyMenu(ecoMatic);
                    break;
                case "2":
                    CustomerBuyMenu(ecoMatic);
                    break;
                case "3":
                    CustomerExamineMenu(ecoMatic);
                    break;
                case "4":
                    CustomerRecycleMenu(ecoMatic);
                    break;
                case "5":
                    ecoMatic.GetChange();
                    return;
                default:
                    Write.Error("Invalid Input");
                    break;
            }
        }
        
    }

    public static void AdminMenu(EcoMatic ecoMatic)
    {
        Write.DelayLoad("Still in development");
    }

    // customer menu methods
    public static void CustomerInsertMoneyMenu(EcoMatic ecoMatic)
    {
        Console.WriteLine("\nAccepted bills by the eco-matic:");
        foreach (decimal bill in ecoMatic.AcceptedBills)
        {
            AnsiConsole.Markup($"[green]₱{bill} [/]");
        }
        Console.WriteLine();

        Console.Write("Enter bill amount to insert: ₱");
        string input = Console.ReadLine() ?? "";
        if (decimal.TryParse(input, out decimal amount))
        {
            ecoMatic.InsertMoney(amount);
        }
        else
        {
            Write.Error("Invalid input.");
        }
    }
    // ccustomer buy menu
    public static void CustomerBuyMenu(EcoMatic ecoMatic)
    {
        Console.WriteLine("\nWhich item would you like to buy? ");
        Console.Write("Input item ID: ");
        string input = Console.ReadLine() ?? "";
        if (int.TryParse(input, out int id))
        {
            ecoMatic.BuyItem(id);
        }
        else
        {
            Write.Error("Invalid input.");
        }
    }

    // customer examine menu
    public static void CustomerExamineMenu(EcoMatic ecoMatic)
    {
        Console.WriteLine("\nWhich item would you like to examine?");
        Console.Write("Input item ID: ");
        string input = Console.ReadLine() ?? "";
        if (int.TryParse(input, out int id))
        {
            ecoMatic.ExamineItem(id);
        }
        else
        {
            Write.Error("Invalid input.");
        }
    }
    
    // customer recycle menu
    public static void CustomerRecycleMenu(EcoMatic ecoMatic)
    {
        Console.WriteLine("\nList of items for recycle and price per gram.");
        for (int i = 0; i < ecoMatic.RecyclableItems.Length; i++)
        {
            AnsiConsole.MarkupLine($"[bold yellow]{i + 1}.[/]{ecoMatic.RecyclableItems[i]} - ₱{ecoMatic.RecycleItemsPricePerGram[i]:F2} per gram");
        }
        Console.WriteLine("Which item would you like to recycle?");
        Console.Write("Input item ID: ");
        string input = Console.ReadLine() ?? "";
        if (int.TryParse(input, out int id))
        {
            Console.Write("Enter weight in grams (1 - 5000g): ");
            string gramsInput = Console.ReadLine() ?? "";
            if (double.TryParse(gramsInput, out double grams))
            {
                ecoMatic.RecycleForCredit(id, grams);
            }
            else
            {
                Write.Error("Invalid input.");
            }
        }
        else
        {
            Write.Error("Invalid Input.");
        }
    }
}

// helper class so i have fewer lines of code when using thread sleep
class Write
{
    public static void DelayLine(string message, int delay = 300)
    {
        Console.WriteLine(message);
        Thread.Sleep(delay);
    }

    public static void Delay(string message, int delay = 300)
    {
        Console.Write(message);
        Thread.Sleep(delay);
    }

    public static void DelayLoad(string message, int delay = 300)
    {
        Write.Delay(message);
        for (int i = 0; i < 3; i++)
        {
            Write.Delay(".", 300);
        }
        Console.Write("\n");
    }
    
    public static void Warning(string message, int delay = 1000)
    {
        AnsiConsole.MarkupLine("[yellow][[WARNING]][/] " + message);
        Thread.Sleep(delay);
    }

    public static void Error(string message, int delay = 1000)
    {
        AnsiConsole.MarkupLine("[red][[ERROR]][/] " + message);
        Thread.Sleep(delay);
    }

    public static void Success(string message, int delay = 1000)
    {
        AnsiConsole.MarkupLine("[green][[SUCCESS]][/] " + message);
        Thread.Sleep(delay);
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

    // Transaction tracking arrays
    private string[] _transactionItemNames = new string[100];
    private decimal[] _transactionPrices = new decimal[100];
    private int[] _transactionQuantities = new int[100];
    private decimal[] _transactionTotalPrices = new decimal[100];
    private int _transactionCount = 0;

    // Recycle transaction tracking arrays
    private string[] _recycleItemNames = new string[100];
    private double[] _recycleWeights = new double[100];
    private decimal[] _recyclePricesPerGram = new decimal[100];
    private decimal[] _recycleCredits = new decimal[100];
    private int _recycleCount = 0;
    
    public decimal CurrentBalance { get; private set; }
    public decimal[] AcceptedBills = { 20, 50, 100, 200, 500, 1000 };

    // recycle items to be implemented in the future - recycleItems, for now it is only stored on memory
    // parallel arrays (per gram pricing)
    public string[] RecyclableItems = { "Plastic Bottle","Glass Bottle" , "Aluminum Can"};
    public decimal[] RecycleItemsPricePerGram = { 0.01m, 0.02m, 0.03m };

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
    // insert money
    public void InsertMoney(decimal amount)
    {
        if (!AcceptedBills.Contains(amount))
        {
            Write.Error("Invalid bill amount.");
            return;
        } 
        CurrentBalance += amount;
        LogEvent("TRANSACTION", "INSERT_MONEY", "", 0, 1, 0, $"Inserted ₱{amount}. Current Balance: ₱{CurrentBalance}");
        Write.Success($"₱{amount} inserted successfully.");
    }
    // buy item
    public void BuyItem(int id)
    {
        id--;
        if (id < 0 || id >= _itemCount)
        {
            Write.Error("Invalid Range.");
            return;
        }

        VendingItem item = _inventory[id];
        if (item.ItemStock <= 0)
        {
            Write.Error($"Sorry, {item.ItemName} is out of stock.");
            return;
        }

        if (CurrentBalance < item.ItemPrice)
        {
            Write.Error("Sorry, you don't have enough balance");
            return;
        }

        AnsiConsole.MarkupLine(item.GetDispenseMessage());
        Write.Success($"You have successfully received {item.ItemName}");
        CurrentBalance -= item.ItemPrice;
        item.ItemStock--;
        UpdateInventory();
        LogEvent("PURCHASE", "BUY_ITEM", item.ItemName, item.ItemPrice, 1, item.ItemPrice, $"Bought 1x {item.ItemName}. Remaining Balance: ₱{CurrentBalance}");
        
        // Add to transaction tracking
        _transactionItemNames[_transactionCount] = item.ItemName;
        _transactionPrices[_transactionCount] = item.ItemPrice;
        _transactionQuantities[_transactionCount] = 1;
        _transactionTotalPrices[_transactionCount] = item.ItemPrice;
        _transactionCount++;
        
        Write.DelayLoad("Thanks for using eco-matic! Returning");
    }
    // examine item
    public void ExamineItem(int id)
    {
        id--;
        if (id < 0 || id >= _itemCount)
        {
            Write.Error("Invalid Range.");
            return;
        }
        VendingItem item = _inventory[id];
        AnsiConsole.MarkupLine(item.GetExamineMessage());
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
    // recylce item
    public void RecycleForCredit(int id, double grams)
    {
        id--;
        if (id < 0 || id >= RecyclableItems.Length)
        {
            Write.Error("Invalid Range.");
            return;
        }
        if (grams <= 0 || grams > 5000)
        {
            Write.Error("Invalid weight. Please enter a value between 1 and 5000 grams.");
            return;
        }

        decimal credit = (decimal)grams * RecycleItemsPricePerGram[id];
        CurrentBalance += credit;
        LogEvent("RECYCLE", "RECYCLE_ITEM", RecyclableItems[id], RecycleItemsPricePerGram[id], (int)grams, credit, $"Recycled {grams}g of {RecyclableItems[id]}. Credited ₱{credit:F2}. Current Balance: ₱{CurrentBalance:F2}");
        
        // Track recycled item
        _recycleItemNames[_recycleCount] = RecyclableItems[id];
        _recycleWeights[_recycleCount] = grams;
        _recyclePricesPerGram[_recycleCount] = RecycleItemsPricePerGram[id];
        _recycleCredits[_recycleCount] = credit;
        _recycleCount++;
        
        Write.Success($"Recycled {grams}g of {RecyclableItems[id]}. Credited ₱{credit:F2}.");
    }
    // get change and get receipt
    public void GetChange()
    {
        if (CurrentBalance <= 0 && _transactionCount == 0)
        {
            Write.Error("No transactions to return. Your balance is ₱0.00");
            return;
        }

        // Calculate total spent
        decimal totalSpent = 0;
        for (int i = 0; i < _transactionCount; i++)
        {
            totalSpent += _transactionTotalPrices[i];
        }

        // Create receipt
        Console.Clear();
        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
        AnsiConsole.MarkupLine("[bold cyan]ECO-MATIC RECEIPT[/]");
        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");

        if (_transactionCount > 0)
        {
            AnsiConsole.MarkupLine("[yellow]Items Purchased:[/]");
            for (int i = 0; i < _transactionCount; i++)
            {
                AnsiConsole.MarkupLine($"{_transactionItemNames[i]} x{_transactionQuantities[i]} ₱{_transactionTotalPrices[i]:F2}");
            }
            AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
            AnsiConsole.MarkupLine($"[cyan]Total Spent:[/] ₱{totalSpent:F2}");
        }

        if (_recycleCount > 0)
        {
            decimal totalRecycleCredit = 0;
            for (int i = 0; i < _recycleCount; i++)
            {
                totalRecycleCredit += _recycleCredits[i];
            }
            
            AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
            AnsiConsole.MarkupLine("[green]Items Recycled:[/]");
            for (int i = 0; i < _recycleCount; i++)
            {
                AnsiConsole.MarkupLine($"{_recycleItemNames[i]} {_recycleWeights[i]}g ₱{_recycleCredits[i]:F2}");
            }
            AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
            AnsiConsole.MarkupLine($"[cyan]Total Recycled Credit:[/] ₱{totalRecycleCredit:F2}");
        }

        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
        AnsiConsole.MarkupLine($"[yellow]Change Amount:[/] ₱{CurrentBalance:F2}");
        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
        AnsiConsole.MarkupLine("[italic]Thank you for using Eco-Matic![/]");
        AnsiConsole.MarkupLine("[italic]Help save the environment today.[/]");
        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");

        if (CurrentBalance > 0)
        {
            Write.DelayLine("Dispensing change...", 500);
            Write.Success($"Dispensed ₱{CurrentBalance:F2}");
            LogEvent("TRANSACTION", "RETURN_CHANGE", "", 0, 1, CurrentBalance, $"Returned ₱{CurrentBalance:F2} to customer.");
        }

        // reset for next customer
        CurrentBalance = 0;
        
        // clear transaction arrays
        for (int i = 0; i < _transactionCount; i++)
        {
            _transactionItemNames[i] = "";
            _transactionPrices[i] = 0;
            _transactionQuantities[i] = 0;
            _transactionTotalPrices[i] = 0;
        }
        _transactionCount = 0;
        
        // clear recycle arrays
        for (int i = 0; i < _recycleCount; i++)
        {
            _recycleItemNames[i] = "";
            _recycleWeights[i] = 0;
            _recyclePricesPerGram[i] = 0;
            _recycleCredits[i] = 0;
        }
        _recycleCount = 0;

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    //show inventory in a table using nuget package spectre console
    public void ShowInventory()
    {
        Console.Clear();

        // create format of table
        Table table = new Table();
        table.Border = TableBorder.Rounded;
        table.BorderColor(Color.Green);
        table.Title = new TableTitle("[bold green]+--- ECO-MATIC VENDING MACHINE ---+[/]");
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
        
        AnsiConsole.MarkupLine($"[bold green]Current Balance: ₱{CurrentBalance:F2}[/]");
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
                Write.Warning($"Directory {_dataDirectory} doesn't exist");
                Write.DelayLoad($"Creating new {_dataDirectory} directory");
                Directory.CreateDirectory(_dataDirectory);
            }
        }
        catch (IOException ex)
        {
            Write.Error("Error checking directory. " + ex.Message);
        }
        
    }

    // check for inventory validity  
    private void CheckInventoryFile()
    {
        string filePath = Path.Combine(_dataDirectory, _inventoryFileName);

        if (!File.Exists(filePath))
        {
            Write.Warning($"{_inventoryFileName} file doesn't exist.");
            Write.DelayLoad($"Creating new {_inventoryFileName} file");
            CreateDefaultInventoryFile();
            return;
        }

        // check header if correct
        string[] lines = File.ReadAllLines(filePath);
        int length = lines.Length;
        if (lines[0] != "Type,Name,Price,Stock,Calories/Volume" || length <= 1)
        {
            Write.Warning($"Something is wrong with {_inventoryFileName}");
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
                Write.Error($"Abnormalities found on line {i + 1}");
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
            Write.Error("Error updating inventory. " + ex.Message);
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
        }
        catch (IOException ex)
        {
            Write.Error("Error updating inventory. " + ex.Message);
        }
    }

    // log event to be added to eventlog csv
    private void LogEvent(string eventType, string action, string itemName, decimal unitPrice, int quantity, decimal totalPrice, string details)
    {
        string filePath = Path.Combine(_dataDirectory, _eventLogFileName);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logLine = $"{timestamp},{eventType},{action},{itemName},{unitPrice},{quantity},{totalPrice},{details}";

        try
        {
            using (StreamWriter w = new StreamWriter(filePath, true))
            {
                w.WriteLine(logLine);
            }
        }
        catch (IOException ex)
        {
            Write.Error("Error logging event. " + ex.Message);
        }
    }

    // checks even log file for errors
    private void CheckEventLogFile()
    {
        string filePath = Path.Combine(_dataDirectory, _eventLogFileName);

        if (!File.Exists(filePath))
        {
            Write.Warning($"{_eventLogFileName} file doesn't exist.");
            Write.DelayLoad($"Creating new {_eventLogFileName} file");
            CreateDefaultEventLogFile();
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length == 0 || lines[0] != "Timestamp,EventType,Action,ItemName,UnitPrice,Quantity,TotalPrice,Details")
        {
            Write.Warning($"Something is wrong with {_eventLogFileName}");
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
            w.WriteLine("Timestamp,EventType,Action,ItemName,UnitPrice,Quantity,TotalPrice,Details");
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
    public string ItemName { get; set; }
    public decimal ItemPrice { get; set; }
    public int ItemStock { get; set; }

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
        return $"[bold green]Munch time![/] Your [yellow]{ItemName}[/] is ready to go!";
    }

    public override string GetExamineMessage()
    {
        return $"[bold cyan]{ItemName}[/] | [green]Calories:[/] {Calories}kcal | [italic]Delicious snack for sustainable energy.[/]";
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
        return $"[bold cyan]{ItemName} ready![/] [green]Sip with purpose.[/]";
    }

    public override string GetExamineMessage()
    {
        return $"[bold cyan]{ItemName}[/] | [blue]Volume:[/] {Volume}ml | [italic]Ice cold refreshment - perfectly sustainable hydration.[/]";
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
        return $"[bold green]{ItemName} acquired![/] [yellow]Keep eco-ing![/]";
    }

    public override string GetExamineMessage()
    {
        return $"[bold green]{ItemName}[/] | [italic]Essential item for eco-conscious living.[/]";
    }

    public override string ToCsvLine()
    {
        return $"Misc,{ItemName},{ItemPrice},{ItemStock},";
    }
}



