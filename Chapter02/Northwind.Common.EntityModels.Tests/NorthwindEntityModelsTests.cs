using Northwind.Common.DataContext.SqlServer;
using Xunit.Abstractions;

namespace Northwind.Common.EntityModels.Tests;

public class NorthwindEntityModelsTests
{
    private ITestOutputHelper output { get; set; }
    public NorthwindEntityModelsTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void CanConnectIsTrue()
    {
        using(SqlServer.NorthwindDb db = new()) //Arrange
        {   
            bool canConnect = db.Database.CanConnect(); //Act

            Assert.True(canConnect);
        }
    }

    [Fact]
    public void ProductIdIsChai()
    {
        using(SqlServer.NorthwindDb db = new()) //Arrange
        {
            SqlServer.Product product1 = db.Products.Single(p=>p.ProductId == 1); //Act

            Assert.Equal("Chai", product1.ProductName);
        }
    }

    [Fact]
    public void EmployeeHasLastRefreshedIn10sWindow()
    {
        using(SqlServer.NorthwindDb db = new())
        {
            SqlServer.Employee employee1 = db.Employees.Single(p=>p.EmployeeId == 1);

            DateTimeOffset now = DateTimeOffset.UtcNow;

            Assert.InRange(actual: employee1.LastRefreshed, low: now.Subtract(TimeSpan.FromSeconds(5)), high: now.AddSeconds(5));

            output.WriteLine("Refreshed: {0}", employee1.LastRefreshed);
        }
    }
}