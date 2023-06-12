using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectDotNetCore.Endpoints.Products;

public class ProductGetAll
{
      public static string Template => "/products";
      public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
      public static Delegate Handle => Action;

      [Authorize(Policy = "UserPolicy")]
      public static async Task<IResult> Action(ApplicationDbContext context)
      {
          var products = context.Products.Include(p => p.CategoryId).OrderBy(p => p.Name).ToList();
          var response = products.Select(p => new ProductResponse(p.Name, p.Category.Name, p.Description, p.HasStock, p.Active));

          return Results.Ok(response);
      }
}
