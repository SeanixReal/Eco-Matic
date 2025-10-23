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
    public double Volume { get; set; }

    public DrinkItem(string itemName, double itemPrice, int itemStock, double volume) : base(itemName, itemPrice, itemStock)
    {
        Volume = volume;
    }

    public override string GetDisplayInfo()
    {
        return "";
    }

    public override string GetDispenseMessage()
    {
        return "";
    }

    public override string GetExamineMessage()
    {
        return "";
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
        return "";
    }

    public override string GetDispenseMessage()
    {
        return "";
    }

    public override string GetExamineMessage()
    {
        return "";
    }
}

class MiscItem : VendingItem
{
    public MiscItem(string itemName, double itemPrice, int itemStock) : base(itemName, itemPrice, itemStock) { }

    public override string GetDisplayInfo()
    {
        return "";
    }

    public override string GetDispenseMessage()
    {
        return "";
    }

    public override string GetExamineMessage()
    {
        return "";
    }
}

class Program
{
    public static void Main(string[] args)
    {

    }

    public static void MainMenu(EcoMatic EcoMatic)
    {

    }

    public static void CustomerMenu(EcoMatic EcoMatic)
    {

    }
    
    public static void AdminMenu(EcoMatic EcoMatic){

    }
}