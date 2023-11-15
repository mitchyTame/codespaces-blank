﻿namespace MappingObjects.Models;

public record class LineItem(
    string ProductName,
    decimal UnitPrice,
    int Quantity
);
