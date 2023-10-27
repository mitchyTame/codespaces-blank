// See https://aka.ms/new-console-template for more information

using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NorthWind.Console.SqlClient;

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

SqlConnection connection = new(builder.ConnectionString);

WriteLine(connection.ConnectionString);
WriteLine();

connection.StateChange += Connection_StateChange;
connection.InfoMessage += Connection_InfoMessage;

try
{
    WriteLine("Opening Connection. Please wait up to {0} seconds...", builder.ConnectTimeout);
    WriteLine();

    await connection.OpenAsync();

    WriteLine($"SQL Server version: {connection.ServerVersion}");

    connection.StatisticsEnabled = true;
}
catch (SqlException ex)
{
    WriteLine($"Sql Exception: {ex.Message}");
    return;
}

Write("Enter a unit price: ");
string? priceText = ReadLine();

if (!decimal.TryParse(priceText, out decimal price))
{
    WriteLine("You must enter a valid unit price. ");
    return;
}

SqlCommand cmd = connection.CreateCommand();

WriteLine("Execute command using: ");
WriteLine(" 1 - Text");
WriteLine(" 2 - Stored Procedure");
WriteLine();
Write("Press a key: ");

key = ReadKey().Key;

WriteLine(); WriteLine();

SqlParameter p1, p2 = new(), p3 = new();

if (key is ConsoleKey.D1 or ConsoleKey.NumPad1)
{
    cmd.CommandType = CommandType.Text;

    cmd.CommandText = "SELECT ProductId, ProductName, UnitPrice FROM Products" + " WHERE UnitPrice > @price";

    cmd.Parameters.AddWithValue("price", price);
}
else if (key is ConsoleKey.D2 or ConsoleKey.NumPad2)
{
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.CommandText = "GetExpensiveProducts";

    p1 = new()
    {
        ParameterName = "price",
        SqlDbType = SqlDbType.Money,
        SqlValue = price
    };

    p2 = new()
    {
        Direction = ParameterDirection.Output,
        ParameterName = "count",
        SqlDbType = SqlDbType.Int
    };

    p3 = new()
    {
        Direction = ParameterDirection.ReturnValue,
        ParameterName = "rv",
        SqlDbType = SqlDbType.Int
    };

    cmd.Parameters.Add(p1);
    cmd.Parameters.Add(p2);
    cmd.Parameters.Add(p3);

}


SqlDataReader r = await cmd.ExecuteReaderAsync();

WriteLine("-----------------------------------------------------------------------");
WriteLine("|  {0,5}  |  {1, -35}  {2,8}", "Id", "Name", "Price");
WriteLine("-----------------------------------------------------------------------");



while (await r.ReadAsync())
{
    WriteLine("|  {0,5}  |  {1, -35} | {2,8:C}  |",
    r.GetInt32("ProductId"),
    r.GetString("ProductName"),
    r.GetDecimal("UnitPrice"));
};


WriteLine("------------------------------------------------------------------------");

await r.CloseAsync();

WriteLine($"Output count: {p2.Value}");
WriteLine($"Return value: {p3.Value}");

IEnumerable<Supplier> suppliers = connection.Query<Supplier>(
    sql: "Select * From suppliers where Country=@Country",
    param: new {Country = "Germany"});

foreach(Supplier supplier in suppliers)
{
    WriteLine("{0}, {1}, {2}, {3}",
    supplier.SupplierId, supplier.CompanyName, supplier.City, supplier.Country
    );
}

await connection.CloseAsync();




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
        ForegroundColor = ConsoleColor.DarkRed;

        WriteLine($"Info: {e.Message}");
        foreach (SqlError error in e.Errors)
        {
            WriteLine($" Error: {error.Message}");
        }
        ForegroundColor = previousColor;
    }
}


