namespace FirstProjectDotNetCore.Endpoints.Products;

public class ProductDto
{
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public string Description { get; set; }
    public bool HasStock { get; set; }
    public bool Active { get; set; }
    public decimal Price { get; set; }
}
