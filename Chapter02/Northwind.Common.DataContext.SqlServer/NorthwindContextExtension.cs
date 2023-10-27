using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Common.EntityModels.SqlServer;

namespace Northwind.Common.DataContext.SqlServer;

public static class NorthwindContextExtension
{
    ///<summary>
    ///Adds NorthwindContext to the specified IServiceCollection. Uses the SQLServer database provider.
    ///</summary>
    ///<param name="connectionString">
    ///Set to Override the default
    /// </param>
    /// <param name="services"></param>
    public static IServiceCollection AddNorthwindContext(this IServiceCollection services, 
    string connectionString = "Server=.;Database=Northwind;" + "User Id=sa;Password=P@ssw0rd; TrustServerCertificate=true;"
    )
    {
        services.AddDbContext<NorthwindDb>(options => {
            options.UseSqlServer(connectionString);
            options.LogTo(Console.WriteLine, new[] {
                Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting
            });
        });

        return services;
    }
}
