namespace Northwind.Common.EntityModels.SqlServer;

public interface IHasLastRefreshed
{
    DateTimeOffset LastRefreshed {get; set;}
}
