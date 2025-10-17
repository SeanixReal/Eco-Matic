using System;
using System.Linq;
using System.IO;
using System.Threading;

// the project Eco-Matic!!
class EcoMatic
{

}

// parent guidelines for items in eco-matic
abstract class VendingItem
{
    public string ItemName { get; set; }
    public double ItemPrice { get; set; }

    public VendingItem(string itemName, double itemPrice)
    {
        ItemName = itemName;
        ItemPrice = itemPrice;
    }

    public abstract string GetDisplayInfo();
    public abstract string GetExamineMessage();
    public abstract string GetDispenseMessage();
}

// interfaces 
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

    public DrinkItem(string itemName, double itemPrice, double volume) : base(itemName, itemPrice)
    {
        Volume = volume;
    }

    public override string GetDisplayInfo()
    {
        throw new NotImplementedException();
    }

    public override string GetDispenseMessage()
    {
        throw new NotImplementedException();
    }

    public override string GetExamineMessage()
    {
        throw new NotImplementedException();
    }
}

class SnackItem : VendingItem, IHasCalories
{
    public double Calories { get; set; }

    public SnackItem(string itemName, double itemPrice, double calories) : base(itemName, itemPrice)
    {
        Calories = calories;
    }

    public override string GetDisplayInfo()
    {
        throw new NotImplementedException();
    }

    public override string GetDispenseMessage()
    {
        throw new NotImplementedException();
    }

    public override string GetExamineMessage()
    {
        throw new NotImplementedException();
    }
}

class MiscItem : VendingItem
{
    public MiscItem(string itemName, double itemPrice) : base(itemName, itemPrice) { }

    public override string GetDisplayInfo()
    {
        throw new NotImplementedException();
    }

    public override string GetDispenseMessage()
    {
        throw new NotImplementedException();
    }

    public override string GetExamineMessage()
    {
        throw new NotImplementedException();
    }
}

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Eco-Matic is still in development...");

        string fp;
        try
        {
            fp = "Inventory.csv";
            if (!File.Exists(fp))
            {
                CreateDefaultInventory(fp);
            }

            fp = "TransactionLog.txt";
            if (!File.Exists(fp))
            {
                CreateDefaultTransactionLog(fp);
            }

            
        } 
        catch (Exception ex)
        {
            Console.WriteLine("Oopsie, something went wrong... " + ex.Message); 
        }

        /* bool flag = true;
        while (flag)
        {
            MainMenu();
        } */
    }

    public static void MainMenu()
    {

    }
    
    public static void CreateDefaultInventory(string fp)
    {
        Console.WriteLine("Creating Inventory.csv with default value...");
        Thread.Sleep(1000);
        using (StreamWriter writer = new StreamWriter(File.Create(fp)))
        {
            writer.WriteLine("Type,ItemName,ItemPrice,Additional");
            writer.WriteLine("Snack,MrChips,30,160");
            writer.WriteLine("Snack,Nova,50,140");
            writer.WriteLine("Drink,Water,25,500");
            writer.WriteLine("Drink,Coca-Cola,30,500");
            writer.WriteLine("Misc,Pen,15,");
            writer.WriteLine("Misc,Wet Wipes,30,");
        }
    }

    public static void CreateDefaultTransactionLog(string fp)
    {
        Console.WriteLine("Creating TransactionLog.txt with default value...");
        Thread.Sleep(1000);
        using (StreamWriter writer = new StreamWriter(File.Create(fp)))
        {
            writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - EVENT: Transaction log initialized.");
        }
    }
}