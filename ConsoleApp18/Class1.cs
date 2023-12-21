public class MenuItem
{
    public string Id { get; }
    public string Description { get; }

    public MenuItem(string id, string description)
    {
        Id = id;
        Description = description;
    }
}
public class FinancialRecord
{
    public string TransactionId { get; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime TransactionDate { get; set; }

    public FinancialRecord(string transactionId, decimal amount, string description, DateTime transactionDate)
    {
        TransactionId = transactionId;
        Amount = amount;
        Description = description;
        TransactionDate = transactionDate;
    }
}

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string FullName { get; set; }
    public string Position { get; set; }
}

public class Employee
{
    public string EmployeeId { get; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }
    public string UserId { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public Employee(string employeeId, string firstName, string lastName, string position, string userId, string login, string password, string role)
    {
        EmployeeId = employeeId;
        FirstName = firstName;
        LastName = lastName;
        Position = position;
        UserId = userId;
        Login = login;
        Password = password;
        Role = role;
    }
}
public class Receipt
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime PurchaseDate { get; set; }
}
public class Accounting
{
    public decimal TotalRevenue { get; set; }
    private static decimal totalRevenue = 0;
    public static void UpdateAccounting(decimal totalPrice)
    {
        totalRevenue += totalPrice;
    }
}
public class Product
{
    public string ProductId { get; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public int StockQuantity { get; set; }
    public Product(string productId, string name, decimal price, int quantity)
    {
        ProductId = productId;
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}