using System;
using System.Linq;
using System.IO;
using System.Threading;

// the project Eco-Matic!!
class EcoMatic
{
    private const int MaxItems = 6; // minumum should be 6
    
    
    public double CurrentBalance { get; private set; }

    private VendingItem[] _inventory = new VendingItem[MaxItems];
    private string[] _recycledItems = new string[3] { "Plastic", "Glass", "Metal" };
    private double[] _recycledItemsPrice = new double[3] { 5, 10, 20 };
    private int _itemCount = 0;
    private string _inventoryFilePath;
    private string _transactionLogFilePath;

    public EcoMatic(string inventoryFp, string transactionLogFp)
    {
        _inventoryFilePath = inventoryFp;
        _transactionLogFilePath = transactionLogFp;
        _inventory[0] = new DrinkItem("Coca Cola", 30, 10, 500);
        _inventory[1] = new DrinkItem("Pepsi", 25, 10, 500);
        _inventory[2] = new SnackItem("Mr Chips", 35, 10, 160);
        _inventory[3] = new SnackItem("Nova", 40, 10, 180);
        _inventory[4] = new MiscItem("Bandage", 5, 10);
        _inventory[5] = new MiscItem("Eco Bag", 10, 10);
        _itemCount = 6;
    }

    public void ShowInventory()
    {
        Console.WriteLine("\nAvailable Items:");
        for (int i = 0; i < _itemCount; i++)
        {
            Console.WriteLine($"{i + 1}. {_inventory[i].GetDisplayInfo()}");
        }
    }
    
    public void Purchase()
    {
        if (CurrentBalance == 0)
        {
            Console.WriteLine("Please insert money first.");
            Thread.Sleep(2000);
            return;
        }

        int itemNumber;
        Console.Write("Select item to purchase (0 to cancel): ");
        while (!int.TryParse(Console.ReadLine(), out itemNumber) || itemNumber < 0 || itemNumber > _itemCount)
        {
            Console.WriteLine("Invalid input. Please try again.");
            Console.Write($"Enter item number (1-{MaxItems}, or 0 to cancel): ");
        }

        if (itemNumber == 0) return;

        int index = itemNumber - 1;
        VendingItem itemChoice = _inventory[index];

        if (itemChoice.ItemStock <= 0)
        {
            Console.WriteLine($"Sorry, {itemChoice.ItemName} is out of stock.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        if (CurrentBalance < itemChoice.ItemPrice)
        {
            Console.WriteLine($"Insufficient balance. {itemChoice.ItemName} costs ₱{itemChoice.ItemPrice:F2}.");
            Console.WriteLine($"Your balance: ₱{CurrentBalance:F2}");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        CurrentBalance -= itemChoice.ItemPrice;
        itemChoice.ItemStock--;
        Console.WriteLine(itemChoice.GetDispenseMessage());
        UpdateTransaction("Purchase", itemChoice.ItemName, itemChoice.ItemPrice);
        UpdateInventory();

        Console.WriteLine($"Remaining balance: ₱{CurrentBalance:F2}");
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }
    
    public void Recycle()
    {
        Console.WriteLine("Recycle for Cash!");
        for (int i = 0; i < _recycledItems.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {_recycledItems[i]} (₱{_recycledItemsPrice[i]:F2} per gram)");
        }

        Console.Write("\nSelect item type to recycle (0 to cancel): ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > _recycledItems.Length)
        {
            Console.WriteLine("Invalid input. Please try again.");
            Console.Write($"Select item type (1-{_recycledItems.Length}, or 0 to cancel): ");
        }

        if (choice == 0) return;

        int index = choice - 1;
        Console.Write($"Enter weight of {_recycledItems[index]} to recycle (grams): ");

        double grams;
        while (!double.TryParse(Console.ReadLine(), out grams) || grams <= 0)
        {
            Console.WriteLine("Invalid weight. Please enter a positive number.");
            Console.Write("Weight (grams): ");
        }

        double total = grams * _recycledItemsPrice[index];
        CurrentBalance += total;

        Console.WriteLine($"Thank you for recycling {grams} grams of {_recycledItems[index]}!");
        Console.WriteLine($"You earned ₱{total:F2}");
        Console.WriteLine($"New balance: ₱{CurrentBalance:F2}");

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    public void Examine()
    {
        int itemNumber;
        Console.Write("Enter item number to examine (0 to cancel): ");
        while (!int.TryParse(Console.ReadLine(), out itemNumber) || itemNumber < 0 || itemNumber > MaxItems)
        {
            Console.WriteLine("Invalid input. Please try again.");
            Console.Write($"Enter item number (1-{MaxItems}, or 0 to cancel): ");
        }
        
        if (itemNumber == 0) return;
        
        int index = itemNumber - 1;
        VendingItem itemChoice = _inventory[index];
        
        Console.WriteLine($"\nExamining: {itemChoice.ItemName}");
        Console.WriteLine($"Price: ₱{itemChoice.ItemPrice:F2}");
        Console.WriteLine($"Stock: {itemChoice.ItemStock} available");
        Console.WriteLine($"Details: {itemChoice.GetExamineMessage()}");
        
        Console.WriteLine("\nPress Enter to continue.");
        Console.ReadLine();
    }

    public void InsertMoney()
    {
        double amount;
        Console.Write("Enter amount to insert (₱5-₱100, or 0 to cancel): ");
        while (!double.TryParse(Console.ReadLine(), out amount) || (amount != 0 && (amount < 5 || amount > 100)))
        {
            Console.WriteLine("Invalid amount. Please enter ₱5-₱100.");
            Console.Write("Amount (or 0 to cancel): ");
        }
        
        if (amount == 0) return;
        
        CurrentBalance += amount;
        Console.WriteLine($"₱{amount:F2} inserted successfully!");
        Console.WriteLine($"Current balance: ₱{CurrentBalance:F2}");
        
        Console.WriteLine("\nPress Enter to continue.");
        Console.ReadLine();
    }
    
    public void GetChange()
    {
        if (CurrentBalance == 0)
        {
            Console.WriteLine("No balance to return.");
            return;
        }
        
        Console.WriteLine($"Returning change: ₱{CurrentBalance:F2}");
        CurrentBalance = 0;
        Console.WriteLine("Thank you for using Eco-Matic!");
    }
    
    public void Restock()
    {
        Console.Clear();
        ShowInventory();
        Console.WriteLine("Restock Item");
        Console.Write("Enter item number to restock (0 to cancel): ");
        int itemNumber;
        while (!int.TryParse(Console.ReadLine(), out itemNumber) || itemNumber < 0 || itemNumber > _itemCount)
        {
            Console.WriteLine("Invalid input. Please try again.");
            Console.Write($"Enter item number (1-{_itemCount}, or 0 to cancel): ");
        }

        if (itemNumber == 0) return;

        int index = itemNumber - 1;
        VendingItem itemToRestock = _inventory[index];

        Console.Write($"Enter quantity to add to {itemToRestock.ItemName}'s stock: ");
        int quantity;
        while (!int.TryParse(Console.ReadLine(), out quantity) || quantity < 1)
        {
            Console.WriteLine("Invalid quantity. Please enter a positive number.");
            Console.Write("Quantity: ");
        }

        itemToRestock.ItemStock += quantity;
        Console.WriteLine($"{quantity} units added to {itemToRestock.ItemName}. New stock: {itemToRestock.ItemStock}");

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    public void AddItem()
    {
        Console.Clear();
        Console.WriteLine("Add New Item");

        if (_itemCount >= MaxItems)
        {
            Console.WriteLine("Inventory is full. Cannot add more items.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        Console.Write("Enter item name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Enter item price: ");
        double price;
        while (!double.TryParse(Console.ReadLine(), out price) || price <= 0)
        {
            Console.WriteLine("Invalid price. Please enter a positive number.");
            Console.Write("Price: ");
        }

        Console.Write("Enter item stock: ");
        int stock;
        while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0)
        {
            Console.WriteLine("Invalid stock. Please enter a non-negative number.");
            Console.Write("Stock: ");
        }

        Console.WriteLine("Select item type:");
        Console.WriteLine("1. Drink");
        Console.WriteLine("2. Snack");
        Console.WriteLine("3. Miscellaneous");
        Console.Write("Choice: ");
        int typeChoice;
        while (!int.TryParse(Console.ReadLine(), out typeChoice) || typeChoice < 1 || typeChoice > 3)
        {
            Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
            Console.Write("Choice: ");
        }

        VendingItem newItem = null;
        switch (typeChoice)
        {
            case 1:
                Console.Write("Enter volume (ml): ");
                double volume;
                while (!double.TryParse(Console.ReadLine(), out volume) || volume <= 0)
                {
                    Console.WriteLine("Invalid volume. Please enter a positive number.");
                    Console.Write("Volume (ml): ");
                }
                newItem = new DrinkItem(name, price, stock, volume);
                break;
            case 2:
                Console.Write("Enter calories: ");
                double calories;
                while (!double.TryParse(Console.ReadLine(), out calories) || calories <= 0)
                {
                    Console.WriteLine("Invalid calories. Please enter a positive number.");
                    Console.Write("Calories: ");
                }
                newItem = new SnackItem(name, price, stock, calories);
                break;
            case 3:
                newItem = new MiscItem(name, price, stock);
                break;
        }

        if (newItem != null)
        {
            _inventory[_itemCount] = newItem;
            _itemCount++;
            Console.WriteLine($"{newItem.ItemName} added to inventory.");
        }
        else
        {
            Console.WriteLine("Failed to add item.");
        }

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    public void RemoveItem()
    {
        Console.Clear();
        ShowInventory();
        Console.WriteLine("Remove Item");
        Console.Write("Enter item number to remove (0 to cancel): ");
        int itemNumber;
        while (!int.TryParse(Console.ReadLine(), out itemNumber) || itemNumber < 0 || itemNumber > _itemCount)
        {
            Console.WriteLine("Invalid input. Please try again.");
            Console.Write($"Enter item number (1-{_itemCount}, or 0 to cancel): ");
        }

        if (itemNumber == 0) return;

        int index = itemNumber - 1;
        VendingItem itemToRemove = _inventory[index];

        for (int i = index; i < _itemCount - 1; i++)
        {
            _inventory[i] = _inventory[i + 1];
        }
        _inventory[_itemCount - 1] = null;
        _itemCount--;

        Console.WriteLine($"{itemToRemove.ItemName} removed from inventory.");

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
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

    private void UpdateTransaction(string eventName, string name, double total)
    {
        
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