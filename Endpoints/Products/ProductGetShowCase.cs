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
    public static async Task<IResult> Action(ApplicationDbContext context, int page = 1, int row = 1, string orderBy = "Name")
     {
          if (row > 10) {
            return Results.Problem(title: "Rows with max 10", statusCode: 400);
          }
          var queryBase = context.Products.AsNoTracking().Include(p => p.Category)
              .Where(p => p.HasStock && p.Category.Active);

          var queryFilter = queryBase.Skip((page - 1) * row).Take(row);
          if (orderBy == "Name")
          {
              queryFilter = queryFilter.OrderBy(p => p.Name);
          }
          else {
              queryFilter = queryFilter.OrderBy(p => p.Price);
          }

          var products = queryFilter.ToList();

          var response = products.Select(p => new ProductResponse(p.Name, p.Category.Name, p.Description,p.Price, p.HasStock, p.Active));

          return Results.Ok(response);
      }
}
