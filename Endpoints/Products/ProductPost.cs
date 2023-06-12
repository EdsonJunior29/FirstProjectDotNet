using FirstProjectDotNetCore.Domain.Products;
using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FirstProjectDotNetCore.Endpoints.Products;

public class ProductPost
{
    public static string Template => "/products";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "UserPolicy02")]
    public static async Task<IResult> Action(ProductDto productDto, HttpContext http, ApplicationDbContext context)
        {
            //obter informações do usuário que está autenticado
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == productDto.CategoryId);

            var product = new Product(productDto.Name, category, productDto.Description,productDto.Price, productDto.HasStock, userId);

            if (!product.IsValid)
            {
                return Results.ValidationProblem(product.Notifications.ConvertToProblemDetails());
            }
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return Results.Created($"/product/{product.Id}", product.Id);
        }

}
