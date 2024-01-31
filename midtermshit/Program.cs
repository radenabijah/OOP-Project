using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

// Abstract base class representing an inventory item
abstract class InventoryItem
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpirationDate { get; set; }

    // Constructor to initialize the inventory item
    public InventoryItem(string name, decimal price, int quantity, DateTime expirationDate)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
        ExpirationDate = expirationDate;
    }

    // Virtual method to display information about the inventory item
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Price (per kilo): {Price:C}");
        Console.WriteLine($"Quantity (in kilo): {Quantity}");
        Console.WriteLine($"Expiration Date: {ExpirationDate.ToShortDateString()}");
    }
}

// Derived class representing a meat item in the inventory
class MeatItem : InventoryItem
{
    public MeatItem(string name, decimal price, int quantity, DateTime expirationDate)
        : base(name, price, quantity, expirationDate)
    {
    }
}

// Interface defining operations that can be performed on the inventory
interface IInventoryOperations
{
    void CreateItem(InventoryItem item);
    void ReadItems();
    void UpdateItem(string name, int quantity, decimal price, DateTime expirationDate);
    void DeleteItem(string name);
}

// Interface defining sales operations
interface ISalesOperations
{
    void RecordSale(DateTime date, decimal amount);
    void DisplaySalesReports();
}

// Class representing a sales report
class SalesReport
{
    private Dictionary<DateTime, decimal> dailySales;
    private Dictionary<int, decimal> weeklySales;
    private Dictionary<int, decimal> monthlySales;

    // Constructor to initialize dictionaries for daily, weekly, and monthly sales
    public SalesReport()
    {
        dailySales = new Dictionary<DateTime, decimal>();
        weeklySales = new Dictionary<int, decimal>();
        monthlySales = new Dictionary<int, decimal>();
    }

    // Method to record daily sales
    public void RecordDailySale(DateTime date, decimal amount)
    {
        if (dailySales.ContainsKey(date))
        {
            dailySales[date] += amount;
        }
        else
        {
            dailySales.Add(date, amount);
        }
    }

    // Method to record weekly sales
    public void RecordWeeklySale(int weekNumber, decimal amount)
    {
        if (weeklySales.ContainsKey(weekNumber))
        {
            weeklySales[weekNumber] += amount;
        }
        else
        {
            weeklySales.Add(weekNumber, amount);
        }
    }

    // Method to record monthly sales
    public void RecordMonthlySale(int month, decimal amount)
    {
        if (monthlySales.ContainsKey(month))
        {
            monthlySales[month] += amount;
        }
        else
        {
            monthlySales.Add(month, amount);
        }
    }

    // Method to display daily sales report
    public void DisplayDailySales()
    {
        Console.WriteLine("\nDaily Sales Report:");
        foreach (var entry in dailySales)
        {
            Console.WriteLine($"{entry.Key.ToShortDateString()}: {entry.Value:C}");
        }
    }

    // Method to display weekly sales report
    public void DisplayWeeklySales()
    {
        Console.WriteLine("\nWeekly Sales Report:");
        foreach (var entry in weeklySales)
        {
            Console.WriteLine($"Week {entry.Key}: {entry.Value:C}");
        }
    }

    // Method to display monthly sales report
    public void DisplayMonthlySales()
    {
        Console.WriteLine("\nMonthly Sales Report:");
        foreach (var entry in monthlySales)
        {
            Console.WriteLine($"Month {entry.Key}: {entry.Value:C}");
        }
    }
}

// Class representing a receipt for a purchased item
class Receipt
{
    public string ItemName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime PurchaseDate { get; set; }

    // Constructor to initialize the receipt
    public Receipt(string itemName, decimal quantity, decimal unitPrice, decimal totalPrice, DateTime purchaseDate)
    {
        ItemName = itemName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
        PurchaseDate = purchaseDate;
    }

    // Method to display the receipt
    public void DisplayReceipt()
    {
        Console.WriteLine("\nReceipt:");
        Console.WriteLine($"Item: {ItemName}");
        Console.WriteLine($"Quantity: {Quantity} kilos");
        Console.WriteLine($"Unit Price: {UnitPrice:C}");
        Console.WriteLine($"Total Price: {TotalPrice:C}");
        Console.WriteLine($"Purchase Date: {PurchaseDate.ToShortDateString()} at {PurchaseDate.ToShortTimeString()}");
        Console.WriteLine("Thank you for purchasing at MeatMart.");
    }
}

// Class representing an owner with inventory management capabilities
class Owner : IInventoryOperations, ISalesOperations
{
    private List<InventoryItem> inventory;
    private SalesReport salesReport;

    private string inventoryFileName = "inventory.txt";

    // Constructor to initialize inventory and sales report, and load initial inventory from file
    public Owner()
    {
        inventory = new List<InventoryItem>();
        inventory.Add(new MeatItem("Chicken", 205.00m, 8, DateTime.Now.AddDays(5)));
        inventory.Add(new MeatItem("Beef", 280.00m, 9, DateTime.Now.AddDays(7)));
        salesReport = new SalesReport();

        // Load initial inventory from file
        LoadInventoryFromFile();
    }

    // Method to load inventory from a file
    private void LoadInventoryFromFile()
    {
        try
        {
            if (File.Exists(inventoryFileName))
            {
                string[] lines = File.ReadAllLines(inventoryFileName);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 5)
                    {
                        string name = parts[0];
                        decimal price = decimal.Parse(parts[1]);
                        int quantity = int.Parse(parts[2]);
                        DateTime expirationDate = DateTime.Parse(parts[3]);
                        MeatItem meatItem = new MeatItem(name, price, quantity, expirationDate);
                        inventory.Add(meatItem);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading inventory from file: {ex.Message}");
        }
    }

    // Method to save inventory to a file
    public void SaveInventoryToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(inventoryFileName))
            {
                foreach (InventoryItem item in inventory)
                {
                    writer.WriteLine($"{item.Name},{item.Price},{item.Quantity},{item.ExpirationDate}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving inventory to file: {ex.Message}");
        }
    }

    // Method to create an inventory item
    public void CreateItem(InventoryItem item)
    {
        inventory.Add(item);
        SaveInventoryToFile(); // Save to file after creating an item
    }

    // Method to display all inventory items
    public void ReadItems()
    {
        Console.WriteLine("Inventory Items:\n");
        foreach (var item in inventory)
        {
            item.DisplayInfo();
            Console.WriteLine();
        }
    }

    // Method to update an inventory item
    public void UpdateItem(string name, int quantity, decimal price, DateTime expirationDate)
    {
        var item = inventory.Find(i => i.Name == name);
        if (item != null)
        {
            item.Quantity = quantity;
            item.Price = price;
            item.ExpirationDate = expirationDate;
        }
        else
        {
            Console.WriteLine("Item not found in inventory.");
        }
    }

    // Method to delete an inventory item
    public void DeleteItem(string name)
    {
        var item = inventory.Find(i => i.Name == name);
        if (item != null)
        {
            inventory.Remove(item);
            Console.WriteLine("Item deleted from inventory.");
        }
        else
        {
            Console.WriteLine("Item not found in inventory.");
        }
    }

    // Method to get available meat products from the inventory
    public List<MeatItem> GetAvailableMeatProducts()
    {
        return inventory.OfType<MeatItem>().ToList();
    }

    // Method to deduct the quantity of a purchased product from the inventory
    public void DeductQuantity(string productName, decimal purchasedQuantity)
    {
        var meatProduct = inventory.OfType<MeatItem>().FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        if (meatProduct != null)
        {
            meatProduct.Quantity = (int)(meatProduct.Quantity - purchasedQuantity);
        }
    }

    // Implement methods from ISalesOperations
    public void RecordSale(DateTime date, decimal amount)
    {
        salesReport.RecordDailySale(date, amount);

        // Assuming each week starts on Sunday
        int weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        salesReport.RecordWeeklySale(weekNumber, amount);

        salesReport.RecordMonthlySale(date.Month, amount);
    }

    public void DisplaySalesReports()
    {
        salesReport.DisplayDailySales();
        salesReport.DisplayWeeklySales();
        salesReport.DisplayMonthlySales();
    }
}

// Class representing a customer with purchasing capabilities
class Customer
{
    // Method for a customer to buy a product
    public void BuyProduct(Owner owner, InventoryItem item, decimal quantityInKilos)
    {
        decimal totalPrice = item.Price * quantityInKilos;
        Console.WriteLine($"Customer bought {quantityInKilos} kilos of {item.Name} for {totalPrice:C}");
        owner.DeductQuantity(item.Name, quantityInKilos);
        owner.RecordSale(DateTime.Now, totalPrice); // Record the sale

        // Generate and display a receipt
        Receipt receipt = new Receipt(item.Name, quantityInKilos, item.Price, totalPrice, DateTime.Now);
        receipt.DisplayReceipt();
    }

    // Method to display available meat products to a customer
    public void DisplayAvailableMeatProducts(List<MeatItem> meatProducts)
    {
        Console.WriteLine("Available Meat Products:");
        foreach (var product in meatProducts)
        {
            product.DisplayInfo();
            Console.WriteLine();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Owner owner = new Owner();
            Customer customer = new Customer();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\nEnter 'O' for Owner or 'C' for Customer:");
                string userType = Console.ReadLine()?.ToUpper() ?? "";

                if (userType == "O")
                {
                    OwnerMenu(owner);
                }
                else if (userType == "C")
                {
                    CustomerMenu(customer, owner);
                }
                else
                {
                    Console.WriteLine("Invalid user type. 'O' for Owner or 'C' for Customer only.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    // Method to display the owner menu
    static void OwnerMenu(Owner owner)
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("\nOwner Menu:");
            Console.WriteLine("1. Create Item");
            Console.WriteLine("2. Read Items");
            Console.WriteLine("3. Update Item");
            Console.WriteLine("4. Delete Item");
            Console.WriteLine("5. View Sales Reports");
            Console.WriteLine("6. Back to Main Menu");
            Console.WriteLine("7. Exit\n");

            try
            {
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter item name: ");
                        string name = Console.ReadLine();
                        Console.Write("Enter price (per kilo): ");
                        decimal price = decimal.Parse(Console.ReadLine());
                        Console.Write("Enter quantity (in kilo): ");
                        int quantity = int.Parse(Console.ReadLine());
                        Console.Write("Enter expiration date (yyyy-MM-dd): ");
                        DateTime expirationDate = DateTime.Parse(Console.ReadLine());
                        MeatItem meatItem = new MeatItem(name, price, quantity, expirationDate);
                        owner.CreateItem(meatItem);
                        Console.WriteLine("Item added to inventory.");
                        break;
                    case 2:
                        owner.ReadItems();
                        break;
                    case 3:
                        Console.Write("Enter item name to update: ");
                        string updateName = Console.ReadLine();
                        Console.Write("Enter new price (per kilo): ");
                        decimal updatePrice = decimal.Parse(Console.ReadLine());
                        Console.Write("Enter new quantity (in kilo): ");
                        int updateQuantity = int.Parse(Console.ReadLine());
                        Console.Write("Enter new expiration date (yyyy-MM-dd): ");
                        DateTime updateExpirationDate = DateTime.Parse(Console.ReadLine());
                        owner.UpdateItem(updateName, updateQuantity, updatePrice, updateExpirationDate);
                        Console.WriteLine("Item updated from inventory.");
                        break;
                    case 4:
                        Console.Write("Enter item name to delete: ");
                        string deleteName = Console.ReadLine();
                        owner.DeleteItem(deleteName);
                        break;
                    case 5:
                        owner.DisplaySalesReports();
                        break;
                    case 6:
                        owner.SaveInventoryToFile(); // Save inventory before going back to the main menu
                        Console.WriteLine("Inventory saved to file.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return;
                    case 7:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input format. Please enter a number.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    // Method to display the customer menu
    static void CustomerMenu(Customer customer, Owner owner)
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("\nCustomer Menu:");
            Console.WriteLine("1. Display Available Meat Products");
            Console.WriteLine("2. Buy Product");
            Console.WriteLine("3. Back to Main Menu");
            Console.WriteLine("4. Exit\n");

            try
            {
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        List<MeatItem> availableMeatProducts = owner.GetAvailableMeatProducts();
                        customer.DisplayAvailableMeatProducts(availableMeatProducts);
                        break;
                    case 2:
                        Console.Write("Enter the name of the meat product you want to buy: ");
                        string productName = Console.ReadLine();
                        MeatItem meatProductToBuy = owner.GetAvailableMeatProducts().FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));

                        if (meatProductToBuy != null)
                        {
                            Console.Write($"Enter the quantity (in kilos) of {meatProductToBuy.Name} to buy: ");
                            decimal quantityToBuy = decimal.Parse(Console.ReadLine());

                            if (quantityToBuy <= meatProductToBuy.Quantity)
                            {
                                customer.BuyProduct(owner, meatProductToBuy, quantityToBuy);
                            }
                            else
                            {
                                Console.WriteLine("Not enough quantity available for purchase.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Meat product not found.");
                        }
                        break;
                    case 3:
                        return;
                    case 4:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input format. Please enter a number.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
