using Microsoft.Extensions.Configuration;
using TryAzureCosmos.Entities;
using TryAzureCosmos.Utils;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = configuration["AzureCosmos:ConnectionString"];

if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentNullException(nameof(connectionString));
}

var dbUtil = new DBUtil(connectionString, "TEAMModelOS");
var containerName = "Test";


var item = new Product(
    id: "a64321ce-ed53-4208-bb24-29877becce41",
    categoryId: "0078cde9-3989-4ee0-ab0c-eaa0c047900b",
    name: "product 2",
    price: 850.00m,
    tags: ["tag1", "tag2"]
);

//await dbUtil.UpsertItem(containerName, item, new PartitionKey(item.categoryId));

//var product = await dbUtil.ReadItem<Product>(containerName, item.id, new PartitionKey(item.categoryId));
//Console.WriteLine(product);

//await dbUtil.DeleteItem<Product>(containerName, item.id, new PartitionKey(item.categoryId));

var parameters = new Dictionary<string, object>
{
    {"@categoryId", "26C74104-40BC-4541-8EF5-9892F7F03D72"},
};
var products = await dbUtil.Query<Product>(containerName, "SELECT * FROM products p WHERE p.categoryId = @categoryId", parameters);


Console.WriteLine("Done!");

