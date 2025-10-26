using System;
using System.Linq;
using System.IO;
using System.Threading;

// the project Eco-Matic!!
class EcoMatic
{
    public double CurrentBalance { get; set; }
    private VendingItem[] _invetory = new VendingItem[6];
    private int _itemCount = 0;
    private string _inventoryFilePath;
    private string _transactionLogFilePath;

    public EcoMatic(string inventoryFp, string transactionLogFp)
    {
        _inventoryFilePath = inventoryFp;
        _transactionLogFilePath = transactionLogFp;
        // initialize inventory 
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

    }
    
    private void UpdateInventory()
    {

    }

    private void UpdateTranscaction(string eventName, string name, double total)
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
    public double Volume { get; set;}

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

            MainMenu(ecoMatic);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Oopsie... Something went wrong: " + ex.Message);
        }

    }

    public static void MainMenu(EcoMatic ecoMatic)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("==== Eco-Matic Vending Machine ====");
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Buy");
            Console.WriteLine("2. Admin");
            Console.WriteLine("3. Exit");
            Console.Write("Select option: ");
            string input = Console.ReadLine() ?? "";
            switch (input)
            {
                case "1":
                    CustomerMenu(ecoMatic);
                    break;
                case "2":
                    AdminMenu(ecoMatic);
                    break;
                case "3":
                    Console.WriteLine("Exiting Eco-Matic. Goodbye!");
                    Thread.Sleep(1000);
                    return;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    public static void CustomerMenu(EcoMatic ecoMatic)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("==== Customer Mode ====");
            ecoMatic.ShowInventory();
            Console.WriteLine($"Current Balance: ₱{ecoMatic.CurrentBalance:F2}");
            Console.WriteLine("\nCustomer Menu:");
            Console.WriteLine("1. Insert Money");
            Console.WriteLine("2. Select Item");
            Console.WriteLine("3. Examine Item");
            Console.WriteLine("4. Recycle");
            Console.WriteLine("5. Finish (Return Change)");
            Console.Write("Select option: ");
            string input = Console.ReadLine() ?? "";
            switch (input)
            {
                case "1":
                    ecoMatic.InsertMoney();
                    break;
                case "2":
                    ecoMatic.Purchase();
                    break;
                case "3":
                    ecoMatic.Examine();
                    break;
                case "4":
                    ecoMatic.Recycle();
                    break;
                case "5":
                    ecoMatic.GetChange();
                    Console.WriteLine("Returning to Main Menu. Press Enter to continue.");
                    Console.ReadLine();
                    return;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    public static void AdminMenu(EcoMatic ecoMatic)
    {
        Console.Clear();
        Console.WriteLine("==== Admin Mode ====");
        Console.Write("Enter admin code: ");
        string code = Console.ReadLine() ?? "";

        if (code != "1234") 
        {
            Console.WriteLine("Access Denied. Press Enter to return to Main Menu.");
            Console.ReadLine();
            return;
        }
        while (true)
        {
            Console.Clear();
            Console.WriteLine("==== Admin Menu ====");
            Console.WriteLine("1. Restock Item");
            Console.WriteLine("2. Add Item");
            Console.WriteLine("3. Remove Item");
            Console.WriteLine("4. View Log");
            Console.WriteLine("5. Clear Log");
            Console.WriteLine("6. Return to Main Menu");
            Console.Write("Select option: ");
            string input = Console.ReadLine() ?? "";
            switch (input)
            {
                case "1":
                    ecoMatic.Restock();
                    break;
                case "2":
                    ecoMatic.AddItem();
                    break;
                case "3":
                    ecoMatic.RemoveItem();
                    break;
                case "4":
                    ecoMatic.ViewLog();
                    break;
                case "5":
                    ecoMatic.ClearLog();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }
}