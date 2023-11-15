using AutoMapper;
using MappingObjects.Mappers;
using MappingObjects.Models;

Cart cart = new(
    Customer: new (
        FirstName: "John",
        LastName: "Smith"
    ),
    Items: new()
    {
        new (ProductName: "Apples", UnitPrice: 0.49M, Quantity: 10),
        new (ProductName: "Bananas", UnitPrice: 0.99M, Quantity: 4)
    }
);

WriteLine($"{cart.Customer}");

foreach(LineItem item in cart.Items)
{
    WriteLine($"   {item}");
}

MapperConfiguration config = CartToSummaryMapper.GetMapperConfiguration();

IMapper mapper = config.CreateMapper();

Summary summary = mapper.Map<Cart, Summary>(cart);

WriteLine($"Summary: {summary.FullName} spent {summary.Total}");
