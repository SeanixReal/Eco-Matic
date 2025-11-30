using System;
using System.Linq;
using System.Threading;
using System.IO;
using Spectre.Console;

class Program
{
    public static void Main(string[] args)
    {
        AnsiConsole.Clear();
        Console.Title = "Eco-Matic Vending Machine";
        try
        {
            EcoMatic ecoMatic = new EcoMatic("inventory.csv", "eventLog.csv", "data");

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
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Main Menu[/]")
                    .PageSize(4)
                    .AddChoices(new[]
                    {
                        "Customer",
                        "Admin",
                        "Exit"
                    })
            );

            switch (choice)
            {
                case "Customer":
                    CustomerMenu(ecoMatic);
                    break;
                case "Admin":
                    AdminMenu(ecoMatic);
                    break;
                case "Exit":
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
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Customer Menu[/]")
                    .PageSize(6)
                    .AddChoices(new[]
                    {
                        "Insert Money",
                        "Buy Item",
                        "Examine Item",
                        "Recycle Item",
                        "Get Change and Return",
                    })
            );

            switch (choice)
            {
                case "Insert Money":
                    CustomerInsertMoneyMenu(ecoMatic);
                    break;
                case "Buy Item":
                    CustomerBuyMenu(ecoMatic);
                    break;
                case "Examine Item":
                    CustomerExamineMenu(ecoMatic);
                    break;
                case "Recycle Item":
                    CustomerRecycleMenu(ecoMatic);
                    break;
                case "Get Change and Return":
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
        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter Password: ").Secret()
        );
        if (password != "SeanixGwapo")
        {
            Write.Error("Invalid Password.");
            return;
        }
        while (true)
        {
            ecoMatic.ShowInventory();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Admin Menu[/]")
                    .PageSize(8)
                    .AddChoices(new[]
                    {
                        "View Sales Report",
                        "Restock Item",
                        "Add Item",
                        "Remove Item",
                        "View Event Log",
                        "Clear Event Log",
                        "Exit Admin"
                    })
            );

            switch (choice)
            {
                case "View Sales Report":
                    AdminSalesReportMenu(ecoMatic);
                    break;
                case "Restock Item":
                    AdminRestockMenu(ecoMatic);
                    break;
                case "Add Item":
                    AdminAddItemMenu(ecoMatic);
                    break;
                case "Remove Item":
                    AdminRemoveItemMenu(ecoMatic);
                    break;
                case "View Event Log":
                    AdminViewLogMenu(ecoMatic);
                    break;
                case "Clear Event Log":
                    AdminClearLogMenu(ecoMatic);
                    break;
                case "Exit Admin":
                    return;
                default:
                    Write.Error("Invalid Input");
                    break;
            }
        }
    }

    public static void AdminSalesReportMenu(EcoMatic ecoMatic)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold yellow]Sales Report[/]")
                .PageSize(6)
                .AddChoices(new[]
                {
                    "Daily",
                    "Weekly",
                    "Monthly",
                    "Yearly",
                    "Cancel"
                })
        );

        switch (choice)
        {
            case "Daily":
                ecoMatic.GenerateDailySalesReport();
                break;
            case "Weekly":
                ecoMatic.GenerateWeeklySalesReport();
                break;
            case "Monthly":
                ecoMatic.GenerateMonthlySalesReport();
                break;
            case "Yearly":
                ecoMatic.GenerateYearlySalesReport();
                break;
            case "Cancel":
                return;
            default:
                Write.Error("Invalid Input");
                break;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static void AdminRestockMenu(EcoMatic ecoMatic)
    {
        ecoMatic.ShowInventory();
        Console.WriteLine("\nWhich item would you like to restock?");
        Console.Write("Input item ID: ");
        string input = Console.ReadLine() ?? "";
        if (int.TryParse(input, out int id))
        {
            ecoMatic.Restock(id);
        }
        else
        {
            Write.Error("Invalid input.");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static void AdminViewLogMenu(EcoMatic ecoMatic)
    {
        ecoMatic.ViewEventLog();
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static void AdminClearLogMenu(EcoMatic ecoMatic)
    {
        Console.Write("Are you sure you want to clear the event log? (yes/no): ");
        string confirm = Console.ReadLine() ?? "";
        if (confirm.ToLower() == "yes")
        {
            ecoMatic.ClearEventLog();
            Write.Success("Event log cleared.");
        }
        else
        {
            Write.Warning("Clear log cancelled.");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static void AdminAddItemMenu(EcoMatic ecoMatic)
    {
        // Check if inventory is already at max before prompting for details
        if (ecoMatic.ItemCount >= EcoMatic.MaxItems)
        {
            Write.Error($"Inventory is full! Cannot add more items. Maximum: {EcoMatic.MaxItems} items");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nAdd New Item");
        Console.Write("Item Type (snack/drink/misc): ");
        string type = Console.ReadLine() ?? "";
        
        Console.Write("Item Name: ");
        string name = Console.ReadLine() ?? "";
        
        Console.Write("Item Price: ₱");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Write.Error("Invalid price.");
            return;
        }
        
        string extraValue = "";
        if (type.ToLower() == "snack")
        {
            Console.Write("Calories: ");
            extraValue = Console.ReadLine() ?? "";
        }
        else if (type.ToLower() == "drink")
        {
            Console.Write("Volume (ml): ");
            extraValue = Console.ReadLine() ?? "";
        }
        
        ecoMatic.AddItem(type, name, price, extraValue);
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static void AdminRemoveItemMenu(EcoMatic ecoMatic)
    {
        ecoMatic.ShowInventory();
        Console.WriteLine("\nWhich item would you like to remove?");
        Console.Write("Input item ID: ");
        string input = Console.ReadLine() ?? "";
        if (int.TryParse(input, out int id))
        {
            ecoMatic.RemoveItem(id);
        }
        else
        {
            Write.Error("Invalid input.");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

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

    public static void CustomerRecycleMenu(EcoMatic ecoMatic)
    {
        Console.WriteLine("\nList of items for recycle and price per gram.");
        for (int i = 0; i < ecoMatic.RecyclableItems.Length; i++)
        {
            AnsiConsole.MarkupLine($"[bold yellow]{i + 1}.[/]{ecoMatic.RecyclableItems[i]} - ₱{ecoMatic.RecycleItemsPricePerGram[i]:F2} per gram");
        }
        Console.WriteLine("Which item would you like to recycle?");
        Console.Write("Input item number: ");
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
    public const int MaxItems = 15;
    public const int MaxStocks = 10;

    private string _inventoryFileName;
    private string _eventLogFileName;
    private string _dataDirectory;

    private VendingItem[] _inventory = new VendingItem[MaxItems];
    public int ItemCount = 0;

    private TransactionTracker _transactionTracker = new TransactionTracker();
    private RecycleTracker _recycleTracker = new RecycleTracker();
    
    public decimal CurrentBalance { get; private set; }
    public decimal[] AcceptedBills = { 20, 50, 100, 200, 500, 1000 };

    public string[] RecyclableItems = { "Plastic Bottle","Glass Bottle" , "Aluminum Can"};
    public decimal[] RecycleItemsPricePerGram = { 0.01m, 0.02m, 0.03m };

    public EcoMatic(string inventoryName, string eventLogName, string directoryFP)
    {
        _inventoryFileName = inventoryName;
        _eventLogFileName = eventLogName;
        _dataDirectory = directoryFP;

        CheckDirectory();
        CheckFiles();

        LoadInventory();
        UpdateInventory();
    }

    public void InsertMoney(decimal amount)
    {
        if (!IsAcceptedBill(amount))
        {
            Write.Error("Invalid bill amount.");
            return;
        } 
        CurrentBalance += amount;
        LogEvent("TRANSACTION", "INSERT_MONEY", "", 0, 1, 0, $"Inserted ₱{amount}. Current Balance: ₱{CurrentBalance}");
        Write.Success($"₱{amount} inserted successfully.");
    }

    public void BuyItem(int id)
    {
        id--;
        if (id < 0 || id >= ItemCount)
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
        
        _transactionTracker.Add(item.ItemName, item.ItemPrice, 1);
        Thread.Sleep(500);
    }

    public void ExamineItem(int id)
    {
        id--;
        if (id < 0 || id >= ItemCount)
        {
            Write.Error("Invalid Range.");
            return;
        }
        VendingItem item = _inventory[id];
        AnsiConsole.MarkupLine(item.GetExamineMessage());
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

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
        
        _recycleTracker.Add(RecyclableItems[id], grams, RecycleItemsPricePerGram[id], credit);
        
        Write.Success($"Recycled {grams}g of {RecyclableItems[id]}. Credited ₱{credit:F2}.");
    }

    public void GetChange()
    {
        bool hasTransactions = _transactionTracker.Count > 0 || _recycleTracker.Count > 0 || CurrentBalance > 0;
        if (!hasTransactions)
        {
            Write.Error("No transactions to return. Your balance is ₱0.00");
            return;
        }

        ReceiptPrinter.Print(_transactionTracker, _recycleTracker, CurrentBalance);

        if (CurrentBalance > 0)
        {
            Write.DelayLine("Dispensing change...", 500);
            Write.Success($"Dispensed ₱{CurrentBalance:F2}");
            LogEvent("TRANSACTION", "RETURN_CHANGE", string.Empty, 0, 1, CurrentBalance, $"Returned ₱{CurrentBalance:F2} to customer.");
        }

        CurrentBalance = 0;
        _transactionTracker.Clear();
        _recycleTracker.Clear();

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public void Restock(int id)
    {
        id--;
        if (id < 0 || id >= ItemCount)
        {
            Write.Error("Invalid ID");
            return;
        }
        VendingItem item = _inventory[id];
        
        int quantityToAdd = MaxStocks - item.ItemStock;
        
        if (quantityToAdd <= 0)
        {
            Write.Warning($"{item.ItemName} is already at max stock ({MaxStocks})");
            return;
        }
        
        item.ItemStock = MaxStocks;
        UpdateInventory();
        Write.Success($"Restocked {item.ItemName} to max capacity. Added {quantityToAdd} units. New stock: {item.ItemStock}/{MaxStocks}");
        LogEvent("RESTOCK", "RESTOCK_ITEM", item.ItemName, 0, quantityToAdd, 0, $"Restocked {item.ItemName} to max. Added {quantityToAdd} units.");
    }

    public void GenerateDailySalesReport()
    {
        SalesReport report = new SalesReport(Path.Combine(_dataDirectory, _eventLogFileName));
        report.PrintRangeReport(DateTime.Now.Date, DateTime.Now.Date, $"DAILY SALES REPORT - {DateTime.Now:yyyy-MM-dd}");
    }

    public void GenerateWeeklySalesReport()
    {
        SalesReport report = new SalesReport(Path.Combine(_dataDirectory, _eventLogFileName));
        DateTime end = DateTime.Now.Date;
        DateTime start = end.AddDays(-6); // last 7 days including today
        report.PrintRangeReport(start, end, $"WEEKLY SALES REPORT - {start:yyyy-MM-dd} to {end:yyyy-MM-dd}");
    }

    public void GenerateMonthlySalesReport()
    {
        SalesReport report = new SalesReport(Path.Combine(_dataDirectory, _eventLogFileName));
        DateTime end = DateTime.Now.Date;
        DateTime start = new DateTime(end.Year, end.Month, 1);
        report.PrintRangeReport(start, end, $"MONTHLY SALES REPORT - {start:yyyy-MM-dd} to {end:yyyy-MM-dd}");
    }

    public void GenerateYearlySalesReport()
    {
        SalesReport report = new SalesReport(Path.Combine(_dataDirectory, _eventLogFileName));
        DateTime end = DateTime.Now.Date;
        DateTime start = new DateTime(end.Year, 1, 1);
        report.PrintRangeReport(start, end, $"YEARLY SALES REPORT - {start:yyyy-MM-dd} to {end:yyyy-MM-dd}");
    }

    public void ViewEventLog()
    {
        AnsiConsole.Clear();
        string filePath = Path.Combine(_dataDirectory, _eventLogFileName);
        if (!File.Exists(filePath))
        {
            Write.Error("Event log file not found.");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        if (lines.Length <= 1)
        {
            AnsiConsole.MarkupLine("[yellow]Event log is empty.[/]");
            return;
        }

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn("Timestamp");
        table.AddColumn("EventType");
        table.AddColumn("Action");
        table.AddColumn("ItemName");
        table.AddColumn("UnitPrice");
        table.AddColumn("Quantity");
        table.AddColumn("TotalPrice");
        table.AddColumn("Details");

        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
            if (parts.Length < 8)
                continue;

            table.AddRow(
                $"[grey]{parts[0]}[/]",
                $"[yellow]{parts[1]}[/]",
                $"[cyan]{parts[2]}[/]",
                $"[white]{parts[3]}[/]",
                $"[green]{parts[4]}[/]",
                $"[blue]{parts[5]}[/]",
                $"[green]{parts[6]}[/]",
                $"[white]{parts[7]}[/]"
            );
        }

        AnsiConsole.Write(table);
    }

    public void ClearEventLog()
    {
        string filePath = Path.Combine(_dataDirectory, _eventLogFileName);
        try
        {
            using (StreamWriter w = new StreamWriter(filePath))
            {
                w.WriteLine("Timestamp,EventType,Action,ItemName,UnitPrice,Quantity,TotalPrice,Details");
            }
        }
        catch (IOException ex)
        {
            Write.Error("Error clearing event log. " + ex.Message);
        }
    }

    public void AddItem(string type, string name, decimal price, string extraValue)
    {
        int stock = MaxStocks; // default stock to MaxStocks constant

        if (ItemCount >= MaxItems)
        {
            Write.Error($"Cannot add more items. Inventory is full (max {MaxItems} items).");
            return;
        }

        type = type.ToLower().Trim();
        if (type == "snack")
        {
            if (!double.TryParse(extraValue, out double calories))
            {
                Write.Error("Invalid calories value.");
                return;
            }
            _inventory[ItemCount++] = new SnackItem(name, price, stock, calories);
        }
        else if (type == "drink")
        {
            if (!double.TryParse(extraValue, out double volume))
            {
                Write.Error("Invalid volume value.");
                return;
            }
            _inventory[ItemCount++] = new DrinkItem(name, price, stock, volume);
        }
        else if (type == "misc")
        {
            _inventory[ItemCount++] = new MiscItem(name, price, stock);
        }
        else
        {
            Write.Error("Invalid item type. Use: snack, drink, or misc");
            return;
        }

        UpdateInventory();
        Write.Success($"Item '{name}' added successfully.");
        LogEvent("ADMIN", "ADD_ITEM", name, price, stock, 0, $"Added new {type} item: {name} at ₱{price}");
    }

    public void RemoveItem(int id)
    {
        id--;
        if (id < 0 || id >= ItemCount)
        {
            Write.Error("Invalid ID");
            return;
        }

        string itemName = _inventory[id].ItemName;
        
        // shift items to the left
        for (int i = id; i < ItemCount - 1; i++)
        {
            _inventory[i] = _inventory[i + 1];
        }
        ItemCount--;

        UpdateInventory();
        Write.Success($"Item '{itemName}' removed successfully.");
        LogEvent("ADMIN", "REMOVE_ITEM", itemName, 0, 0, 0, $"Removed item: {itemName}");
    }

    public void ShowInventory()
    {
        AnsiConsole.Clear();

        // create format of table
        Table table = new Table();
        table.Border = TableBorder.Rounded;
        table.BorderColor(Color.Green);
        table.Title = new TableTitle("[bold green]+--- ECO-MATIC VENDING MACHINE ---+[/]");
        table.AddColumn(new TableColumn("[bold yellow]ID[/]").Centered());
        table.AddColumn(new TableColumn("[bold yellow]Item[/]").LeftAligned());
        table.AddColumn(new TableColumn("[bold yellow]Price[/]").Centered());
        table.AddColumn(new TableColumn("[bold yellow]Stock[/]").Centered());
        table.AddColumn(new TableColumn("[bold yellow]Status[/]").Centered());

        // set up the data to the table to be printed  after
        for (int i = 0; i < ItemCount; i++)
        {
            VendingItem item = _inventory[i];
            
            string stockDisplay = $"[blue]{item.ItemStock}/{MaxStocks}[/]";

            string statusDots;
            if (item.ItemStock == 0)
                statusDots = "[red]OUT OF STOCK[/]";
            else if (item.ItemStock <= (MaxStocks * 0.3)) 
                statusDots = "[red]●[/]";
            else if (item.ItemStock <= (MaxStocks * 0.6)) 
                statusDots = "[yellow]● ●[/]";
            else 
                statusDots = "[green]● ● ●[/]";
            
            table.AddRow(
                $"[cyan]{i + 1}[/]", // item num
                $"[bold]{item.ItemName}[/]", // name
                $"[green]₱{item.ItemPrice:F2}[/]", //price
                stockDisplay, // stock display with numbers
                statusDots // status dots
            );
        }

        AnsiConsole.Write(table);
        
        AnsiConsole.MarkupLine($"[bold green]Current Balance: ₱{CurrentBalance:F2}[/]");
    }

    //File Handling Methods
    private void CheckFiles()
    {
        CheckInventoryFile();
        CheckEventLogFile();
    }
    
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

        string[] lines = File.ReadAllLines(filePath);
        int length = lines.Length;
        if (lines[0] != "Type,Name,Price,Stock,Calories/Volume" || length <= 1)
        {
            Write.Warning($"Something is wrong with {_inventoryFileName}");
            Write.DelayLoad($"Fixing {_inventoryFileName}");
            CreateDefaultInventoryFile();
            return;
        }

        for (int i = 1; i < length; i++)
        {
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
                w.WriteLine("Snack,Piattos,35,10,150");
                w.WriteLine("Snack,Chippy,32,10,170");
                w.WriteLine("Snack,Roller Coaster,28.50,10,140");
                w.WriteLine("Snack,Fudge Bar,25,10,120");
                w.WriteLine("Snack,Cheese Ring,30,10,160");
                w.WriteLine("Drink,RC Cola,25,10,500");
                w.WriteLine("Drink,Sting,27.50,10,500");
                w.WriteLine("Drink,Zest-O Orange,15,10,250");
                w.WriteLine("Drink,Del Monte Pineapple Juice,22.50,10,250");
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
                _inventory[ItemCount++] = new SnackItem(itemName, itemPrice, itemStock, calories);
            }
            else if (itemType == "drink")
            {
                double volume = double.Parse(parts[4]);
                _inventory[ItemCount++] = new DrinkItem(itemName, itemPrice, itemStock, volume);
            }
            else if (itemType == "misc")
            {
                _inventory[ItemCount++] = new MiscItem(itemName, itemPrice, itemStock);
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

                for (int i = 0; i < ItemCount; i++)
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

    private void CreateDefaultEventLogFile()
    {
        string filePath = Path.Combine(_dataDirectory, _eventLogFileName);
        using (StreamWriter w = new StreamWriter(filePath))
        {
            w.WriteLine("Timestamp,EventType,Action,ItemName,UnitPrice,Quantity,TotalPrice,Details");
        }
    }

    private bool IsAcceptedBill(decimal amount)
    {
        for (int i = 0; i < AcceptedBills.Length; i++)
        {
            if (AcceptedBills[i] == amount)
            {
                return true;
            }
        }
        return false;
    }
}

class TransactionTracker
{
    private const int MaxEntries = 100;
    private string[] _itemNames = new string[MaxEntries];
    private decimal[] _unitPrices = new decimal[MaxEntries];
    private int[] _quantities = new int[MaxEntries];
    private decimal[] _lineTotals = new decimal[MaxEntries];
    private int _count = 0;

    public int Count
    {
        get { return _count; }
    }

    public void Add(string itemName, decimal unitPrice, int quantity)
    {
        int existingIndex = FindItemIndex(itemName);
        if (existingIndex >= 0)
        {
            _quantities[existingIndex] += quantity;
            _lineTotals[existingIndex] += unitPrice * quantity;
            _unitPrices[existingIndex] = unitPrice;
            return;
        }

        if (_count >= MaxEntries)
        {
            Write.Warning("Transaction tracker is full. Only the first 100 entries will be shown.");
            return;
        }

        _itemNames[_count] = itemName;
        _unitPrices[_count] = unitPrice;
        _quantities[_count] = quantity;
        _lineTotals[_count] = unitPrice * quantity;
        _count++;
    }

    public string GetItemName(int index)
    {
        return _itemNames[index];
    }

    public int GetQuantity(int index)
    {
        return _quantities[index];
    }

    public decimal GetLineTotal(int index)
    {
        return _lineTotals[index];
    }

    public decimal CalculateTotalSpent()
    {
        decimal total = 0;
        for (int i = 0; i < _count; i++)
        {
            total += _lineTotals[i];
        }
        return total;
    }

    public void Clear()
    {
        for (int i = 0; i < _count; i++)
        {
            _itemNames[i] = "";
            _unitPrices[i] = 0;
            _quantities[i] = 0;
            _lineTotals[i] = 0;
        }
        _count = 0;
    }

    private int FindItemIndex(string itemName)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_itemNames[i] == itemName)
            {
                return i;
            }
        }
        return -1;
    }
}

class RecycleTracker
{
    private const int MaxEntries = 100;
    private string[] _itemNames = new string[MaxEntries];
    private double[] _weights = new double[MaxEntries];
    private decimal[] _pricePerGram = new decimal[MaxEntries];
    private decimal[] _credits = new decimal[MaxEntries];
    private int _count = 0;

    public int Count
    {
        get { return _count; }
    }

    public void Add(string itemName, double weight, decimal pricePerGram, decimal credit)
    {
        int existingIndex = FindItemIndex(itemName);
        if (existingIndex >= 0)
        {
            _weights[existingIndex] += weight;
            _credits[existingIndex] += credit;
            _pricePerGram[existingIndex] = pricePerGram;
            return;
        }

        if (_count >= MaxEntries)
        {
            Write.Warning("Recycle tracker is full. Only the first 100 entries will be shown.");
            return;
        }

        _itemNames[_count] = itemName;
        _weights[_count] = weight;
        _pricePerGram[_count] = pricePerGram;
        _credits[_count] = credit;
        _count++;
    }

    public string GetItemName(int index)
    {
        return _itemNames[index];
    }

    public double GetWeight(int index)
    {
        return _weights[index];
    }

    public decimal GetCredit(int index)
    {
        return _credits[index];
    }

    public decimal CalculateTotalCredits()
    {
        decimal total = 0;
        for (int i = 0; i < _count; i++)
        {
            total += _credits[i];
        }
        return total;
    }

    public void Clear()
    {
        for (int i = 0; i < _count; i++)
        {
            _itemNames[i] = "";
            _weights[i] = 0;
            _pricePerGram[i] = 0;
            _credits[i] = 0;
        }
        _count = 0;
    }

    private int FindItemIndex(string itemName)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_itemNames[i] == itemName)
            {
                return i;
            }
        }
        return -1;
    }
}

class ReceiptPrinter
{
    public static void Print(TransactionTracker transactionTracker, RecycleTracker recycleTracker, decimal changeAmount)
    {
        AnsiConsole.Clear();
        decimal totalSpent = transactionTracker.CalculateTotalSpent();
        decimal totalRecycleCredit = recycleTracker.CalculateTotalCredits();

        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
        AnsiConsole.MarkupLine("[bold cyan]ECO-MATIC RECEIPT[/]");
        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");

        if (transactionTracker.Count > 0)
        {
            AnsiConsole.MarkupLine("[yellow]Items Purchased:[/]");
            for (int i = 0; i < transactionTracker.Count; i++)
            {
                AnsiConsole.MarkupLine($"{transactionTracker.GetItemName(i)} x{transactionTracker.GetQuantity(i)} ₱{transactionTracker.GetLineTotal(i):F2}");
            }
            AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
            AnsiConsole.MarkupLine($"[cyan]Total Spent:[/] ₱{totalSpent:F2}");
        }

        if (recycleTracker.Count > 0)
        {
            AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
            AnsiConsole.MarkupLine("[green]Items Recycled:[/]");
            for (int i = 0; i < recycleTracker.Count; i++)
            {
                AnsiConsole.MarkupLine($"{recycleTracker.GetItemName(i)} {recycleTracker.GetWeight(i)}g ₱{recycleTracker.GetCredit(i):F2}");
            }
            AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
            AnsiConsole.MarkupLine($"[cyan]Total Recycled Credit:[/] ₱{totalRecycleCredit:F2}");
        }

        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
        AnsiConsole.MarkupLine($"[yellow]Change Amount:[/] ₱{changeAmount:F2}");
        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
        AnsiConsole.MarkupLine("[italic]Thank you for using Eco-Matic![/]");
        AnsiConsole.MarkupLine("[italic]Help save the environment today.[/]");
        AnsiConsole.MarkupLine("[bold green]+-------------------------------+[/]");
    }
}

// parses the eventlog to look for purchases
class SalesReport
{
    private const int MaxEntries = 100;
    private string _eventLogPath;

    public SalesReport(string eventLogPath)
    {
        _eventLogPath = eventLogPath;
    }

    public void PrintDailyReport(DateTime date)
    {
        PrintRangeReport(date.Date, date.Date, $"DAILY SALES REPORT - {date:yyyy-MM-dd}");
    }

    public void PrintRangeReport(DateTime startDate, DateTime endDate, string title)
    {
        if (!File.Exists(_eventLogPath))
        {
            Write.Error("Event log file is missing.");
            return;
        }

        string[] lines = File.ReadAllLines(_eventLogPath);
        string[] itemNames = new string[MaxEntries];
        int[] quantities = new int[MaxEntries];
        decimal[] totals = new decimal[MaxEntries];
        int entryCount = 0;
        int totalUnitsSold = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length < 7)
            {
                continue;
            }

            if (!DateTime.TryParse(parts[0].Trim(), out DateTime timestamp))
            {
                continue;
            }

            if (timestamp.Date < startDate.Date || timestamp.Date > endDate.Date || parts[1].Trim() != "PURCHASE")
            {
                continue;
            }

            string itemName = parts[3].Trim();
            int quantity = int.Parse(parts[5]);
            decimal total = decimal.Parse(parts[6]);

            totalUnitsSold += quantity;

            int existingIndex = FindItemIndex(itemNames, entryCount, itemName);
            if (existingIndex >= 0)
            {
                quantities[existingIndex] += quantity;
                totals[existingIndex] += total;
            }
            else if (entryCount < MaxEntries)
            {
                itemNames[entryCount] = itemName;
                quantities[entryCount] = quantity;
                totals[entryCount] = total;
                entryCount++;
            }
        }

        AnsiConsole.MarkupLine("\n[bold green]================================[/]");
        AnsiConsole.MarkupLine($"[bold green]{title}[/]");
        AnsiConsole.MarkupLine("[bold green]================================[/]");

        if (entryCount == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No sales in the selected range.[/]");
            return;
        }

        decimal totalRevenue = 0;
        int bestIndex = -1;
        int worstIndex = -1;
        decimal highestRevenue = 0;
        int highestQuantity = 0;
        int lowestQuantity = int.MaxValue;
        int highestRevenueIndex = -1;

        for (int i = 0; i < entryCount; i++)
        {
            AnsiConsole.MarkupLine($"{itemNames[i]} - Qty: {quantities[i]} - ₱{totals[i]:F2}");
            totalRevenue += totals[i];

            if (quantities[i] > highestQuantity)
            {
                highestQuantity = quantities[i];
                bestIndex = i;
            }

            if (quantities[i] < lowestQuantity)
            {
                lowestQuantity = quantities[i];
                worstIndex = i;
            }

            if (totals[i] > highestRevenue)
            {
                highestRevenue = totals[i];
                highestRevenueIndex = i;
            }
        }

        AnsiConsole.MarkupLine("[bold green]================================[/]");
        AnsiConsole.MarkupLine($"[yellow]Total Revenue: ₱{totalRevenue:F2}[/]");
        AnsiConsole.MarkupLine($"[yellow]Total Units Sold:[/] {totalUnitsSold}");
        AnsiConsole.MarkupLine("[bold green]================================[/]");
        if (bestIndex >= 0)
        {
            AnsiConsole.MarkupLine($"[green]Best Selling Item:[/] {itemNames[bestIndex]} (Qty {quantities[bestIndex]})");
        }

        if (worstIndex >= 0)
        {
            AnsiConsole.MarkupLine($"[yellow]Slowest Moving Item:[/] {itemNames[worstIndex]} (Qty {quantities[worstIndex]})");
        }

        if (highestRevenueIndex >= 0)
        {
            AnsiConsole.MarkupLine($"[green]Top Revenue Item:[/] {itemNames[highestRevenueIndex]} (₱{totals[highestRevenueIndex]:F2})");
        }

        AnsiConsole.MarkupLine("[bold green]================================[/]");
    }
    

    private int FindItemIndex(string[] itemNames, int count, string itemName)
    {
        for (int i = 0; i < count; i++)
        {
            if (itemNames[i] == itemName)
            {
                return i;
            }
        }
        return -1;
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



