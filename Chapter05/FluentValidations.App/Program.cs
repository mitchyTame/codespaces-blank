using FluentValidation.Models;
using FluentValidation.Results;
using FluentValidation.Validators;

Order order = new()
{
    OrderId = 10001,
    CustomerName = "Abcdef",
    CustomerEmail = "abc@example.com",
    CustomerLevel = CustomerLevel.Gold,
    OrderDate = new(2022, 12, 1),
    ShipDate = new(2022, 12, 5),
    Total = 49.99M
};

OrderValidator validator = new();

ValidationResult result = validator.Validate(order);

WriteLine($"CustomerName: {order.CustomerName}");
WriteLine($"CustomerEmail: {order.CustomerEmail}");
WriteLine($"CustomerName: {order.CustomerLevel}");
WriteLine($"CustomerEmail: {order.OrderId}");
WriteLine($"CustomerName: {order.OrderDate}");
WriteLine($"CustomerEmail: {order.ShipDate}");
WriteLine($"CustomerName: {order.Total}");
WriteLine();

foreach(var item in result.Errors)
{
    WriteLine($"{item.Severity}: {item.ErrorMessage}");
}



