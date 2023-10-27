using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Northwind.Console.EFCore.Models;

SqlConnectionStringBuilder builder = new();

builder.InitialCatalog = "NorthWind";
builder.MultipleActiveResultSets = true;
builder.Encrypt = true;
builder.TrustServerCertificate = true;
builder.ConnectTimeout = 10;

WriteLine("Connect to:");
WriteLine("  1 - SQL Server on local Machine");
WriteLine("  2 - Azure SQL Database");
WriteLine("  4 - Azure SQL Edge");
Write("Press a key: ");

ConsoleKey key = ReadKey().Key;
WriteLine(); WriteLine();
if (key is ConsoleKey.D1 or ConsoleKey.NumPad1)
{
    builder.DataSource = ".";
}
else if (key is ConsoleKey.D2 or ConsoleKey.NumPad2)
{
    builder.DataSource = "tcp:apps-services-net7.database.windows.net, 1433";
}
else if (key is ConsoleKey.D3 or ConsoleKey.NumPad3)
{
    builder.DataSource = "tcp: 127.0.0.1, 1433";
}
else
{
    WriteLine("No data source selected.");
    return;
}

WriteLine("Autheticate using: ");
WriteLine("  1 - Windows Integrated Security");
WriteLine("  2 - SQL Login, for example, sa.");
WriteLine();

Write("Press a key: ");
key = ReadKey().Key;

WriteLine(); WriteLine();

if (key is ConsoleKey.D1 or ConsoleKey.NumPad1)
{
    builder.IntegratedSecurity = true;
}
else if (key is ConsoleKey.D2 or ConsoleKey.NumPad2)
{
    builder.UserID = "sa";
    Write("Enter your SQL server password: ");
    string? password = ReadLine();
    if (string.IsNullOrWhiteSpace(password))
    {
        WriteLine("Password cannot be empty or null.");
        return;
    }

    builder.Password = password;
    builder.PersistSecurityInfo = false;
}
else
{
    WriteLine("No authentication selected. ");
    return;
}

DbContextOptionsBuilder<NorthwindDb> options = new();
options.UseSqlServer(builder.ConnectionString);

using (NorthwindDb db = new(options.Options))
{
    Write("Enter a unit price: ");
    string? priceText = ReadLine();

    if(!decimal.TryParse(priceText, out decimal price))
    {
        WriteLine("You must enter a valid unit price");
        return;
    }

    var products = db.Products.Where(p=>p.UnitPrice > price)
    .Select(p=> new {p.ProductId, p.ProductName, p.UnitPrice});

    
WriteLine("-----------------------------------------------------------------------");
WriteLine("|  {0,5}  |  {1, -35}  {2,8}", "Id", "Name", "Price");
WriteLine("-----------------------------------------------------------------------");

foreach(var p in products)
{
    WriteLine("|  {0, 5}   |  {1, -35}  |  {2, 8:C}  |", p.ProductId, p.ProductName, p.UnitPrice);
}

WriteLine("------------------------------------------------------------------------");

WriteLine(products.ToQueryString());
WriteLine();
WriteLine($"Provider: {db.Database.ProviderName}");
WriteLine($"Connection: {db.Database.GetConnectionString()}");

}

partial class Program
{
    static void Connection_StateChange(object sender, StateChangeEventArgs e)
    {
        ConsoleColor previousColor = ForegroundColor;
        ForegroundColor = ConsoleColor.DarkYellow;

        WriteLine($"State change from {e.OriginalState} to {e.CurrentState}");
        ForegroundColor = previousColor;
    }

    static void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
        ConsoleColor previousColor = ForegroundColor;
        WriteLine($"Info: {e.Message}");
        foreach (SqlError error in e.Errors)
        {
            WriteLine($" Error: {error.Message}");
        }
        ForegroundColor = previousColor;
    }
}


