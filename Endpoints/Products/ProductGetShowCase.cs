using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectDotNetCore.Endpoints.Products;

public class ProductGetShowCase
{
      public static string Template => "/products/showcase";
      public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
      public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ApplicationDbContext context)
      {
          var products = context.Products.Include(p => p.Category)
            .Where(p => p.HasStock && p.Category.Active)
            .OrderBy(p => p.Name).ToList();
          var response = products.Select(p => new ProductResponse(p.Name, p.Category.Name, p.Description,p.Price, p.HasStock, p.Active));

          return Results.Ok(response);
      }
}
