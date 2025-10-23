using System;
using System.Linq;
using System.IO;
using System.Threading;

// the project Eco-Matic!!
class EcoMatic
{
    public const int MaxItems = 6; // minimum is 6
    public const int MaxStocks = 10;
    public double CurrentBalance { get; set; }
    private VendingItem[] _inventory = new VendingItem[MaxItems];
    private int _itemCount = 0;
    private string _inventoryFilePath;
    private string _transactionLogFilePath;

    public EcoMatic(string inventoryfilePath, string transactionLogfilePath)
    {
        _inventoryFilePath = inventoryfilePath;
        _transactionLogFilePath = transactionLogfilePath;
        CheckFile(_inventoryFilePath);
        CheckFile(_transactionLogFilePath);
        LoadInventory();
    }

    public void ShowInventory()
    {

    }
    
    public void Purchase()
    {

    }
    
    public void Recycle()
    {
        
    }

    public void Examine()
    {
        
    }

    public void InsertMoney()
    {
        
    }

    public void GetChange()
    {

    }
    
    public void Restock()
    {

    }

    public void AddItem()
    {

    }

    public void RemoveItem()
    {

    }

    public void ViewLog()
    {

    }

    public void ClearLog()
    {

    }

    private void LoadInventory()
    {
        string[] lines = File.ReadAllLines(_inventoryFilePath);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            string type = parts[0].Trim();
            string name = parts[1].Trim();
            double price = double.Parse(parts[2].Trim());
            int stock = int.Parse(parts[3].Trim());


            switch (type)
            {
                case "Snack":
                    double calories = double.Parse(parts[4].Trim());
                    _inventory[_itemCount++] = new SnackItem(name, price, stock, calories);
                    break;
                case "Drink":
                    double volume = double.Parse(parts[4].Trim());
                    _inventory[_itemCount++] = new DrinkItem(name, price, stock, volume);
                    break;
                case "Misc":
                    _inventory[_itemCount++] = new MiscItem(name, price, stock);
                    break;
            }
        }
    }

    private void FileNotExist(string filePath)
    {
        Console.WriteLine($"{filePath} file doesn't exist...");
        Thread.Sleep(2000);
        Console.WriteLine($"Creating new {filePath}");
        Thread.Sleep(2000);
        CreateDefaultFile(filePath);
        Console.WriteLine($"Successfully created new {filePath}");
        Thread.Sleep(2000);
    }

    private void FileIsInvalid(string filePath)
    {
        Console.WriteLine($"Something is wrong with {filePath}");
        Thread.Sleep(2000);
        Console.WriteLine($"Recreating new {filePath}");
        Thread.Sleep(2000);
        File.Delete(filePath);
        CreateDefaultFile(filePath);
        Console.WriteLine($"Successfully recreated {filePath}");
        Console.WriteLine();
    }

    private void CheckFile(string filePath)
    {
        if (filePath == _transactionLogFilePath || filePath == _inventoryFilePath)
        {
            if (!File.Exists(filePath))
            {
                FileNotExist(filePath);
            }

            if (!IsFileValid(filePath))
            {
                FileIsInvalid(filePath);
            }
        }
        else
        {
            throw new Exception("File path not recognize.");
        }
    }
    
    private bool IsTypeValid(string type)
    {
        return type == "Snack" || type == "Drink" || type == "Misc";
    }

    private bool IsFileValid(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        if (filePath == _inventoryFilePath)
        {
            if (lines.Length == 0) return false;
            if (lines.Length > MaxItems + 1) return false;
            if (lines[0] != "Type,Name,Price,Stock,Additional") return false;

            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    string[] parts = lines[i].Split(',');
                    
                    string type = parts[0].Trim();
                    if (!IsTypeValid(type)) return false;

                    double price = double.Parse(parts[2].Trim());
                    int stock = int.Parse(parts[3].Trim());

                    if (type == "Snack" || type == "Drink")
                    {
                        double additional = double.Parse(parts[4].Trim());
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Corrupted data detected on line {i + 1}.");
                    Thread.Sleep(2000);
                    return false;
                }
            }
            return true;
        }        
        else if (filePath == _transactionLogFilePath)
        {
            if (lines.Length == 0) return false;
            if (!lines[0].Contains("EVENT: Transaction log created.")) return false;
            return true;
        }
        else
        {
            throw new Exception("File path not recognized.");
        }
    }

    private void CreateDefaultFile(string filePath)
    {
        if (filePath == _transactionLogFilePath)
        {
            using (StreamWriter w = new StreamWriter(filePath))
            {
                w.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - EVENT: Transaction log created.");
            }
        }
        else if (filePath == _inventoryFilePath)
        {
            using (StreamWriter w = new StreamWriter(filePath))
            {
                w.WriteLine("Type,Name,Price,Stock,Additional");
                w.WriteLine("Snack,MrChip,30,10,200");
                w.WriteLine("Snack,Nova,40,10,160");
                w.WriteLine("Drink,Coca-Cola,10,30,500");
                w.WriteLine("Drink,Royal,10,30,500");
                w.WriteLine("Misc,Pen,20,10,");
                w.WriteLine("Misc,Eco-Bag,30,10,");
            }
        }
        else
        {
            throw new Exception("File Path is not recognized.");            
        }

    }
    
    private void UpdateInventory()
    {
        using (StreamWriter w = new StreamWriter(_inventoryFilePath))
        {
            w.WriteLine("Type,Name,Price,Stock,Additional");

            
        }

    }

    private void UpdateTransaction(string eventName, string name, double total)
    {
        // something like {eventname}: {name} purchased for {total}
    }

}

abstract class VendingItem
{
    public string ItemName { get; set; }
    public double ItemPrice { get; set; }
    public int ItemStock { get; set; }

    public VendingItem(string itemName, double itemPrice, int itemStock)
    {
        ItemName = itemName;
        ItemPrice = itemPrice;
        ItemStock = itemStock;
    }

    public abstract string GetDisplayInfo();
    public abstract string GetExamineMessage();
    public abstract string GetDispenseMessage();
}
 
interface IHasCalories
{
    public double Calories { get; set; }
}

interface IHasVolume
{
    public double Volume { get; set; }
}

class DrinkItem : VendingItem, IHasVolume
{
    public double Volume { get; set; }

    public DrinkItem(string itemName, double itemPrice, int itemStock, double volume) : base(itemName, itemPrice, itemStock)
    {
        Volume = volume;
    }

    public override string GetDisplayInfo()
    {
           return $"{ItemName} (₱{ItemPrice:F2}) [{ItemStock} in stock]";
    }

    public override string GetDispenseMessage()
    {
        return $"🥤 Dispensing. Enjoy your {Volume}ml {ItemName}!";
    }

    public override string GetExamineMessage()
    {
        return $"This is a refreshing beverage, providing {Volume}ml of liquid.";
    }
}

class SnackItem : VendingItem, IHasCalories
{
    public double Calories { get; set; }

    public SnackItem(string itemName, double itemPrice, int itemStock,double calories) : base(itemName, itemPrice, itemStock)
    {
        Calories = calories;
    }

    public override string GetDisplayInfo()
    {
           return $"{ItemName} (₱{ItemPrice:F2}) [{ItemStock} in stock]";
    }

    public override string GetDispenseMessage()
    {
        return $"🍿 Crunch! Grab your {ItemName}. ({Calories} Cal)";
    }

    public override string GetExamineMessage()
    {
        return $"This snack contains {Calories} calories. Perfect for a quick energy boost.";
    }
}

class MiscItem : VendingItem
{
    public MiscItem(string itemName, double itemPrice, int itemStock) : base(itemName, itemPrice, itemStock) { }

    public override string GetDisplayInfo()
    {
           return $"{ItemName} (₱{ItemPrice:F2}) [{ItemStock} in stock]";
    }

    public override string GetDispenseMessage()
    {
        return $"🛍️ Dispensing your {ItemName}. Thank you for using Eco-Matic!";
    }

    public override string GetExamineMessage()
    {
        return $"A simple, reusable item designed to support eco-conscious living.";
    }
}

class Program
{

    public static void Main(string[] args)
    {
        try
        {
            EcoMatic ecoMatic = new EcoMatic("Inventory.csv", "TransactionLog.txt");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Oopsie... Something went wrong: " + ex.Message);
        }
        
    }

    public static void MainMenu(EcoMatic ecoMatic)
    {

    }

    public static void CustomerMenu(EcoMatic ecoMatic)
    {

    }
    
    public static void AdminMenu(EcoMatic ecoMatic){

    }
}