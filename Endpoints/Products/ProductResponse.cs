namespace FirstProjectDotNetCore.Endpoints.Products;

public class ProductResponse
{
    public string  Name { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public bool HasStock { get; set; }
    public bool Active { get; set; }
    public decimal Price { get; set; }

    public ProductResponse(string name, string categoryName, string description, decimal price, bool hasStock, bool active)
    {
        Name = name;
        CategoryName = categoryName;
        Description = description;
        Price = price;
        HasStock = hasStock;
        Active = active;
    }
}
