using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Northwind.Common.EntityModels.SqlServer;
using Northwind.Cosmosdb.Items;

namespace Northwind.CosmoDb.SqlApi;

public static class Resources
    {
        private static string endpointUri = "https://localhost:8081";
        private static string primaryKey = @"C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public static async Task CreateCosmosResources()
        {
            ProgramHelpers.SectionTitle("Creating Cosmos Resources");

            CosmosClientOptions options = new ()
            {
                HttpClientFactory = () => new HttpClient(new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true
            };


            try
            {
                using (CosmosClient client = new(
                    accountEndpoint: endpointUri,
                    authKeyOrResourceToken: primaryKey,
                    clientOptions: options
                    ))
                {
                    DatabaseResponse dbResponse = await client.CreateDatabaseIfNotExistsAsync(
                        "Northwind",
                        throughput: 400);
                    string status = dbResponse.StatusCode switch
                    {
                        HttpStatusCode.OK => "exists",
                        HttpStatusCode.Created => "created",
                        _ => "unknown"
                    };

                    WriteLine("Database Id: {0} Status: {1}", arg0: dbResponse.Database.Id, arg1: status);

                    IndexingPolicy indexPolicy = new()
                    {
                        IndexingMode = IndexingMode.Consistent,
                        Automatic = true
                    };

                    ContainerProperties containerProperties = new("Products",
                        partitionKeyPath: "/productId"
                        )
                    {
                        IndexingPolicy = indexPolicy
                    };

                    ContainerResponse containerResponse = await dbResponse.Database.CreateContainerIfNotExistsAsync(
                        containerProperties, throughput: 1000);

                    status = dbResponse.StatusCode switch
                    {
                        HttpStatusCode.OK => "exists",
                        HttpStatusCode.Created => "created",
                        _ => "unknown"
                    };

                    WriteLine("Container Id: {0}, Status: {1}",
                        arg0: dbResponse.Database.Id,
                        arg1: status
                        );

                    Container container = containerResponse.Container;

                    ContainerProperties properties = await container.ReadContainerAsync();

                    WriteLine($"   PartitionKeyPath: {properties.PartitionKeyPath}");
                    WriteLine($"   LastModified: {properties.LastModified}");
                    WriteLine("    IndexingPolicy.IncludePaths: {0}", arg0: string.Join(",", properties.IndexingPolicy.IncludedPaths.Select(path => path.Path)));
                    WriteLine($"   IndexingPolicy: {properties.IndexingPolicy}");
                }
            }
            catch(HttpRequestException ex)
            {
                WriteLine("Error: {0}", arg0: ex.Message);
                WriteLine("Hint: Make sure the Azure Cosmos Emulator is runnning.");
            }
            catch(Exception ex)
            {
                WriteLine("Error: {0} says {1}", arg0: ex.GetType(), arg1: ex.Message);
            }
        }

        public static async Task CreateProductItems()
        {
            ProgramHelpers.SectionTitle("Creating product items");

            CosmosClientOptions options = new ()
            {
                HttpClientFactory = () => new HttpClient(new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true
            };

            double totalCharge = 0.0;

            try{
                using(CosmosClient client = new(
                    accountEndpoint: endpointUri,
                    authKeyOrResourceToken: primaryKey,
                    clientOptions: options
                ))
                {
                    Container container = client.GetContainer(
                        databaseId: "Northwind", containerId: "Products"
                    );

                    using(NorthwindDb db = new())
                    {
                        ProductCosmos[] products = db.Products.Include(p=>p.Category).Include(p=>p.Supplier).Where(p=>(p.Category != null) && (p.Supplier != null)).Select(
                            p=> new ProductCosmos
                            {
                                id=p.ProductId.ToString(),
                                productId=p.ProductId.ToString(),
                                productName = p.ProductName,
                                quantityPerUnit = p.QuantityPerUnit!.ToString(),
                                category = new CategoryCosmos {
                                    categoryId= p.Category!.CategoryId,
                                    categoryName = p.Category.CategoryName,
                                    description = p.Category.Description
                                },
                                supplier = new SupplierCosmos {
                                    supplierId = p.Supplier!.SupplierId,
                                    companyName = p.Supplier.SupplierId.ToString(),
                                    contactName = p.Supplier.ContactName,
                                    contactTitle = p.Supplier.ContactTitle,
                                    address = p.Supplier.Address,
                                    city = p.Supplier.City,
                                    country = p.Supplier.Country,
                                    postalCode = p.Supplier.PostalCode,
                                    region = p.Supplier.Region,
                                    phone = p.Supplier.Phone,
                                    fax = p.Supplier.Fax,
                                    homePage = p.Supplier.HomePage
                                },
                                unitPrice = p.UnitPrice,
                                unitsInStock = p.UnitsInStock,
                                reorderLevel = p.ReorderLevel,
                                unitsOnOrder = p.UnitsOnOrder,
                                discontinued = p.Discontinued

                            }
                        ).ToArray();

                        foreach(ProductCosmos product in products)
                        {
                            try{
                                ItemResponse<ProductCosmos> productResponse = await container.ReadItemAsync<ProductCosmos>(id:product.id, new PartitionKey(product.productId));
                                WriteLine("Item with id: {0} exists. Query consumed {1} RUs.", productResponse.Resource.id, productResponse.RequestCharge);
                                totalCharge += productResponse.RequestCharge;
                            }
                            catch(CosmosException ex)
                            when(ex.StatusCode == HttpStatusCode.NotFound)
                            {
                                ItemResponse<ProductCosmos> productResponse = await container.CreateItemAsync(product);

                                WriteLine("Created item {0}. Insert consumed {1} RU's. ", productResponse.Resource.id, productResponse.RequestCharge);

                                totalCharge+=productResponse.RequestCharge;
                            }
                            catch(Exception ex)
                            {
                                WriteLine("Error: {0} says {1}", arg0: ex.GetType(), arg1: ex.Message);
                            }
                        }
                    };
                }
            }
            catch(HttpRequestException ex)
            {
                WriteLine("Error: {0}", arg0: ex.Message);
                WriteLine("Hint: Make sure the Azure Cosmos emulator is running");
            }
            catch(Exception ex)
            {
                WriteLine("Error: {0} says {1}", arg0: ex.GetType(), arg1: ex.Message);
            }

            WriteLine("Total Requests charge: {0:N2} RUs", totalCharge);

        }
    }
