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
    }
}